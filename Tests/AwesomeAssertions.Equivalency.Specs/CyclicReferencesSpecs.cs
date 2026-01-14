using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Equivalency.Specs;

public class CyclicReferencesSpecs
{
    [Fact]
    public void Graphs_up_to_the_maximum_depth_are_supported()
    {
        var actual = new ClassWithFiniteRecursiveProperty(recursiveDepth: 10);
        var expectation = new ClassWithFiniteRecursiveProperty(recursiveDepth: 10);

        actual.Should().BeEquivalentTo(expectation);
    }

    [Fact]
    public void Graphs_deeper_than_the_maximum_depth_are_not_supported()
    {
        var actual = new ClassWithFiniteRecursiveProperty(recursiveDepth: 11);
        var expectation = new ClassWithFiniteRecursiveProperty(recursiveDepth: 11);

        Action act = () => actual.Should().BeEquivalentTo(expectation);

        act.Should().Throw<XunitException>().WithMessage("*maximum*depth*10*");
    }

    [Fact]
    public void By_default_cyclic_references_are_not_valid()
    {
        var cyclicRoot = new CyclicRoot { Text = "Root" };
        cyclicRoot.Level = new CyclicLevel1 { Text = "Level1", Root = cyclicRoot };
        var cyclicRootDto = new CyclicRootDto { Text = "Root" };
        cyclicRootDto.Level = new CyclicLevel1Dto { Text = "Level1", Root = cyclicRootDto };

        Action act = () => cyclicRoot.Should().BeEquivalentTo(cyclicRootDto);

        act
            .Should().Throw<XunitException>()
            .WithMessage("Expected property cyclicRoot.Level.Root to be*but it contains a cyclic reference*");
    }

    [Fact]
    public void The_cyclic_reference_itself_will_be_compared_using_simple_equality()
    {
        var expectedChild = new Child { Stuff = 1 };
        var expectedParent = new Parent { Child1 = expectedChild };
        expectedChild.Parent = expectedParent;
        var actualChild = new Child { Stuff = 1 };
        var actualParent = new Parent { Child1 = actualChild };

        var act = () => actualParent.Should().BeEquivalentTo(expectedParent, options => options.IgnoringCyclicReferences());

        act.Should().Throw<XunitException>()
            .WithMessage("Expected property actualParent.Child1.Parent to refer to*but found*null*");
    }

    [Fact]
    public void The_cyclic_reference_can_be_ignored_if_the_comparands_point_to_the_same_object()
    {
        var expectedChild = new Child { Stuff = 1 };
        var expectedParent = new Parent { Child1 = expectedChild };
        expectedChild.Parent = expectedParent;
        var actualChild = new Child { Stuff = 1 };
        var actualParent = new Parent { Child1 = actualChild };

        // Connect this child to the same parent as the expectation child
        actualChild.Parent = expectedParent;

        actualParent.Should().BeEquivalentTo(expectedParent, options => options.IgnoringCyclicReferences());
    }

    [Fact]
    public void Two_graphs_with_ignored_cyclic_references_can_be_compared()
    {
        var actual = new Parent();
        actual.Child1 = new Child(actual, 1);
        actual.Child2 = new Child(actual);
        var expected = new Parent();
        expected.Child1 = new Child(expected);
        expected.Child2 = new Child(expected);

        actual.Should().BeEquivalentTo(expected, x => x
            .Excluding(y => y.Child1)
            .IgnoringCyclicReferences());
    }

    private class Parent
    {
        public Child Child1 { get; set; }

        [UsedImplicitly]
        public Child Child2 { get; set; }
    }

    private class Child
    {
        public Child(Parent parent = null, int stuff = 0)
        {
            Parent = parent;
            Stuff = stuff;
        }

        [UsedImplicitly]
        public Parent Parent { get; set; }

        [UsedImplicitly]
        public int Stuff { get; set; }
    }

    [Fact]
    public void Nested_properties_that_are_null_are_not_treated_as_cyclic_references()
    {
        var actual = new CyclicRoot
        {
            Text = null,
            Level = new CyclicLevel1
            {
                Text = null,
                Root = null
            }
        };
        var expectation = new CyclicRootDto
        {
            Text = null,
            Level = new CyclicLevel1Dto
            {
                Text = null,
                Root = null
            }
        };

        actual.Should().BeEquivalentTo(expectation);
    }

    [Fact]
    public void Equivalent_value_objects_are_not_treated_as_cyclic_references()
    {
        var actual = new CyclicRootWithValueObject
        {
            Value = new ValueObject("MyValue"),
            Level = new CyclicLevelWithValueObject
            {
                Value = new ValueObject("MyValue"),
                Root = null
            }
        };

        var expectation = new CyclicRootWithValueObject
        {
            Value = new ValueObject("MyValue"),
            Level = new CyclicLevelWithValueObject
            {
                Value = new ValueObject("MyValue"),
                Root = null
            }
        };

        actual.Should().BeEquivalentTo(expectation);
    }

    [Fact]
    public void Cyclic_references_do_not_trigger_stack_overflows()
    {
        var recursiveClass1 = new ClassWithInfinitelyRecursiveProperty();
        var recursiveClass2 = new ClassWithInfinitelyRecursiveProperty();

        Action act = () => recursiveClass1.Should().BeEquivalentTo(recursiveClass2);

        act.Should().NotThrow<StackOverflowException>();
    }

    [Fact]
    public void Cyclic_references_can_be_ignored_in_equivalency_assertions()
    {
        var recursiveClass1 = new ClassWithFiniteRecursiveProperty(15);
        var recursiveClass2 = new ClassWithFiniteRecursiveProperty(15);

        recursiveClass1.Should().BeEquivalentTo(recursiveClass2, options => options.AllowingInfiniteRecursion());
    }

    [Fact]
    public void Allowing_infinite_recursion_is_reported_in_the_failure_message()
    {
        var recursiveClass1 = new ClassWithFiniteRecursiveProperty(1);
        var recursiveClass2 = new ClassWithFiniteRecursiveProperty(2);

        Action act = () => recursiveClass1.Should().BeEquivalentTo(recursiveClass2,
            options => options.AllowingInfiniteRecursion());

        act.Should().Throw<XunitException>().WithMessage("*Recurse indefinitely*");
    }

    [Fact]
    public void Can_ignore_cyclic_references_for_inequivalency_assertions()
    {
        var recursiveClass1 = new ClassWithFiniteRecursiveProperty(15);
        var recursiveClass2 = new ClassWithFiniteRecursiveProperty(16);

        recursiveClass1.Should().NotBeEquivalentTo(recursiveClass2,
            options => options.AllowingInfiniteRecursion());
    }

    [Fact]
    public void Can_detect_cyclic_references_in_enumerables()
    {
        var instance1 = new SelfReturningEnumerable();
        var instance2 = new SelfReturningEnumerable();
        var actual = new List<SelfReturningEnumerable> { instance1, instance2 };

        Action act = () => actual.Should().BeEquivalentTo(
            [new SelfReturningEnumerable(), new SelfReturningEnumerable()],
            "we want to test the {0} message", "failure");

        act.Should().Throw<XunitException>().WithMessage("*because we want to test the failure message*cyclic reference*");
    }

    public class SelfReturningEnumerable : IEnumerable<SelfReturningEnumerable>
    {
        public IEnumerator<SelfReturningEnumerable> GetEnumerator()
        {
            yield return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [Fact]
    public void Can_detect_cyclic_references_in_nested_objects_referring_to_the_root()
    {
        var company1 = new MyCompany { Name = "Company" };
        var user1 = new MyUser { Name = "User", Company = company1 };
        company1.Logo = new MyCompanyLogo { Url = "blank", Company = company1, CreatedBy = user1 };
        var company2 = new MyCompany { Name = "Company" };
        var user2 = new MyUser { Name = "User", Company = company2 };
        company2.Logo = new MyCompanyLogo { Url = "blank", Company = company2, CreatedBy = user2 };

        company1.Should().BeEquivalentTo(company2, o => o.IgnoringCyclicReferences());
    }

    [Fact]
    public void Allow_ignoring_cyclic_references_in_value_types_compared_by_members()
    {
        var expectation = new ValueTypeCircularDependency { Title = "First" };
        expectation.Next = new ValueTypeCircularDependency { Title = "Second", Previous = expectation };
        var subject = new ValueTypeCircularDependency { Title = "First" };
        subject.Next = new ValueTypeCircularDependency { Title = "SecondDifferent", Previous = subject };

        Action act = () => subject.Should()
            .BeEquivalentTo(expectation, opt => opt
                .ComparingByMembers<ValueTypeCircularDependency>()
                .IgnoringCyclicReferences());

        act.Should().Throw<XunitException>()
            .WithMessage("*subject.Next.Title*SecondDifferent*Second*")
            .Which.Message.Should().NotContain("maximum recursion depth was reached");
    }

    private class ValueTypeCircularDependency
    {
        public string Title { get; set; }

        public ValueTypeCircularDependency Previous { get; set; }

        public ValueTypeCircularDependency Next { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is ValueTypeCircularDependency baseObj && baseObj.Title == Title;
        }

        public override int GetHashCode()
        {
            return Title.GetHashCode();
        }
    }

    private class ClassWithInfinitelyRecursiveProperty
    {
        public ClassWithInfinitelyRecursiveProperty Self
        {
            get { return new ClassWithInfinitelyRecursiveProperty(); }
        }
    }

    private class ClassWithFiniteRecursiveProperty
    {
        private readonly int depth;

        public ClassWithFiniteRecursiveProperty(int recursiveDepth)
        {
            depth = recursiveDepth;
        }

        public ClassWithFiniteRecursiveProperty Self
        {
            get
            {
                return depth > 0
                    ? new ClassWithFiniteRecursiveProperty(depth - 1)
                    : null;
            }
        }
    }
}
