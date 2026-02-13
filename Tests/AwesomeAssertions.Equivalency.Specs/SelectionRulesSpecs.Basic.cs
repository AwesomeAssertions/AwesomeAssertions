using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using JetBrains.Annotations;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Equivalency.Specs;

public partial class SelectionRulesSpecs
{
    public class Basic
    {
        [Fact]
        public void Property_names_are_case_sensitive()
        {
            var subject = new { Name = "John" };
            var other = new { name = "John" };

            Action act = () => subject.Should().BeEquivalentTo(other);

            act.Should().Throw<XunitException>().WithMessage("Expectation*name**other*not have*");
        }

        [Fact]
        public void Field_names_are_case_sensitive()
        {
            var subject = new ClassWithFieldInUpperCase { Name = "John" };
            var other = new ClassWithFieldInLowerCase { name = "John" };

            Action act = () => subject.Should().BeEquivalentTo(other);

            act.Should().Throw<XunitException>().WithMessage("Expectation*name**other*not have*");
        }

        private class ClassWithFieldInLowerCase
        {
            [UsedImplicitly]
#pragma warning disable SA1307
            public string name;
#pragma warning restore SA1307
        }

        private class ClassWithFieldInUpperCase
        {
            [UsedImplicitly]
            public string Name;
        }

        [Fact]
        public void When_a_property_is_an_indexer_it_should_be_ignored()
        {
            var expected = new ClassWithIndexer { Foo = "test" };
            var result = new ClassWithIndexer { Foo = "test" };

            result.Should().BeEquivalentTo(expected);
        }

        public class ClassWithIndexer
        {
            [UsedImplicitly]
            public object Foo { get; set; }

            public string this[int n] => n.ToString(CultureInfo.InvariantCulture);
        }

        [Fact]
        public void When_the_expected_object_has_a_property_not_available_on_the_subject_it_should_throw()
        {
            var subject = new { };
            var other = new
            {
                // ReSharper disable once StringLiteralTypo
                City = "Rijswijk"
            };

            Action act = () => subject.Should().BeEquivalentTo(other);

            act.Should().Throw<XunitException>()
                .WithMessage("Expectation has property City that the other object does not have*");
        }

        [Fact]
        public void When_equally_named_properties_are_type_incompatible_it_should_throw()
        {
            var subject = new { Type = "A" };
            var other = new { Type = 36 };

            Action act = () => subject.Should().BeEquivalentTo(other);

            act.Should().Throw<XunitException>().WithMessage("Expected property subject.Type to be 36, but found*\"A\"*");
        }

        [Fact]
        public void When_multiple_properties_mismatch_it_should_report_all_of_them()
        {
            var subject = new
            {
                Property1 = "A",
                Property2 = "B",
                SubType1 = new { SubProperty1 = "C", SubProperty2 = "D" }
            };
            var other = new
            {
                Property1 = "1",
                Property2 = "2",
                SubType1 = new { SubProperty1 = "3", SubProperty2 = "D" }
            };

            Action act = () => subject.Should().BeEquivalentTo(other);

            act
                .Should().Throw<XunitException>()
                .WithMessage("""
                             *property subject.Property1*to be *"A" *"1"
                             *property subject.Property2*to be *"B" *"2"
                             *property subject.SubType1.SubProperty1*to be *"C" * "3"*
                             """);
        }

        [Fact]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void Including_all_declared_properties_excludes_all_fields()
        {
            var class1 = new ClassWithSomeFieldsAndProperties
            {
                Field1 = "Lorem", Field2 = "ipsum", Field3 = "dolor",
                Property1 = "sit", Property2 = "amet", Property3 = "consectetur"
            };
            var class2 = new ClassWithSomeFieldsAndProperties
            {
                Field1 = "foo",
                Property1 = "sit", Property2 = "amet", Property3 = "consectetur"
            };

            class1.Should().BeEquivalentTo(class2, opts => opts.IncludingAllDeclaredProperties());
        }

        [Fact]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void Including_all_runtime_properties_excludes_all_fields()
        {
            object class1 = new ClassWithSomeFieldsAndProperties
            {
                Field1 = "Lorem", Field2 = "ipsum", Field3 = "dolor",
                Property1 = "sit", Property2 = "amet", Property3 = "consectetur"
            };
            object class2 = new ClassWithSomeFieldsAndProperties
            {
                Property1 = "sit", Property2 = "amet", Property3 = "consectetur"
            };

            class1.Should().BeEquivalentTo(class2, opts => opts.IncludingAllRuntimeProperties());
        }

        [Fact]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void Respecting_the_runtime_type_includes_both_properties_and_fields_included()
        {
            object class1 = new ClassWithSomeFieldsAndProperties { Field1 = "Lorem", Property1 = "sit" };
            object class2 = new ClassWithSomeFieldsAndProperties();

            Action act =
                () => class1.Should().BeEquivalentTo(class2, opts => opts.PreferringRuntimeMemberTypes());

            act.Should().Throw<XunitException>().Which.Message.Should().Contain("Field1").And.Contain("Property1");
        }

        [Fact]
        public void A_nested_class_without_properties_inside_a_collection_is_fine()
        {
            var sut = new List<BaseClassPointingToClassWithoutProperties> { new() { Name = "theName" } };

            sut.Should().BeEquivalentTo([new BaseClassPointingToClassWithoutProperties { Name = "theName" }]);
        }

#if NETCOREAPP3_0_OR_GREATER
        [Fact]
        public void Will_include_default_interface_properties_in_the_comparison()
        {
            var listA = new List<ITest> { new Test { Name = "Test" } };
            List<ITest> listB = [new Test { Name = "Test" }];

            listA.Should().BeEquivalentTo(listB);
        }

        private class Test : ITest
        {
            public string Name { get; set; }
        }

        private interface ITest
        {
            string Name { get; }

            int NameLength => Name.Length;
        }
#endif

        internal class BaseClassPointingToClassWithoutProperties
        {
            [UsedImplicitly]
            public string Name { get; set; }

            [UsedImplicitly]
            public ClassWithoutProperty ClassWithoutProperty { get; } = new();
        }

        internal class ClassWithoutProperty;
    }
}
