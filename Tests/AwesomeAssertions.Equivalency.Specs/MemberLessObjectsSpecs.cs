using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Equivalency.Specs;

public class MemberLessObjectsSpecs
{
    [Fact]
    public void When_asserting_instances_of_an_anonymous_type_having_no_members_are_equivalent_it_should_fail()
    {
        Action act = () => new { }.Should().BeEquivalentTo(new { });

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void When_asserting_instances_of_a_class_having_no_members_are_equivalent_it_should_fail()
    {
        Action act = () => new ClassWithNoMembers().Should().BeEquivalentTo(new ClassWithNoMembers());

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void When_asserting_instances_of_Object_are_equivalent_it_should_fail()
    {
        Action act = () => new object().Should().BeEquivalentTo(new object());

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void When_asserting_instance_of_object_is_equivalent_to_null_it_should_fail_with_a_descriptive_message()
    {
        object actual = new();
        object expected = null;

        Action act = () => actual.Should().BeEquivalentTo(expected, "we want to test the {0} message", "failure");

        act.Should().Throw<XunitException>()
            .WithMessage("*Expected*to be <null>*we want to test the failure message*, but found System.Object*");
    }

    [Fact]
    public void When_asserting_null_is_equivalent_to_instance_of_object_it_should_fail()
    {
        object actual = null;
        object expected = new();

        Action act = () => actual.Should().BeEquivalentTo(expected);

        act.Should().Throw<XunitException>().WithMessage("*Expected*to be System.Object*but found <null>*");
    }

    [Fact]
    public void When_an_type_only_exposes_fields_but_fields_are_ignored_in_the_equivalence_comparision_it_should_fail()
    {
        var object1 = new ClassWithOnlyAField { Value = 1 };
        var object2 = new ClassWithOnlyAField { Value = 101 };

        Action act = () => object1.Should().BeEquivalentTo(object2, opts => opts.IncludingAllDeclaredProperties());

        act.Should().Throw<InvalidOperationException>("the objects have no members to compare.");
    }

    [Fact]
    public void
        When_an_type_only_exposes_properties_but_properties_are_ignored_in_the_equivalence_comparision_it_should_fail()
    {
        var object1 = new ClassWithOnlyAProperty { Value = 1 };
        var object2 = new ClassWithOnlyAProperty { Value = 101 };

        Action act = () => object1.Should().BeEquivalentTo(object2, opts => opts.ExcludingProperties());

        act.Should().Throw<InvalidOperationException>("the objects have no members to compare.");
    }

    [Fact]
    public void When_asserting_instances_of_arrays_of_types_in_System_are_equivalent_it_should_respect_the_runtime_type()
    {
        object actual = Array.Empty<int>();
        object expectation = Array.Empty<int>();

        actual.Should().BeEquivalentTo(expectation);
    }

    [Fact]
    public void When_throwing_on_missing_members_and_there_are_no_missing_members_should_not_throw()
    {
        var subject = new { Version = 2, Age = 36, };
        var expectation = new { Version = 2, Age = 36 };

        subject.Should().BeEquivalentTo(expectation, options => options.ThrowingOnMissingMembers());
    }

    [Fact]
    public void When_throwing_on_missing_members_and_there_is_a_missing_member_should_throw()
    {
        var subject = new { Version = 2 };
        var expectation = new { Version = 2, Age = 36 };

        Action act = () => subject.Should().BeEquivalentTo(expectation,
            options => options.ThrowingOnMissingMembers());

        act.Should().Throw<XunitException>().WithMessage("Expectation has property Age that the other object does not have*");
    }

    [Fact]
    public void When_throwing_on_missing_members_and_there_is_an_additional_property_on_subject_should_not_throw()
    {
        var subject = new { Version = 2, Age = 36, Additional = 13 };
        var expectation = new { Version = 2, Age = 36 };

        subject.Should().BeEquivalentTo(expectation, options => options.ThrowingOnMissingMembers());
    }
}
