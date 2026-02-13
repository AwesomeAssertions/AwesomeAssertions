using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Equivalency.Specs;

public partial class SelectionRulesSpecs
{
    public class Including
    {
        [Fact]
        public void When_specific_properties_have_been_specified_it_should_ignore_the_other_properties()
        {
            var subject = new { Age = 36, Birthdate = new DateTime(1973, 9, 20), Name = "John" };
            var customer = new { Age = 36, Birthdate = new DateTime(1973, 9, 20), Name = "Dennis" };

            subject.Should().BeEquivalentTo(customer, options => options
                .Including(d => d.Age)
                .Including(d => d.Birthdate));
        }

        [Fact]
        public void A_member_included_by_path_is_described_in_the_failure_message()
        {
            var subject = new { Name = "John" };
            var customer = new { Name = "Jack" };

            Action act = () => subject.Should().BeEquivalentTo(customer, options => options.Including(d => d.Name));

            act.Should().Throw<XunitException>().WithMessage("*Include*Name*");
        }

        [Fact]
        public void A_member_included_by_predicate_is_described_in_the_failure_message()
        {
            var subject = new { Name = "John" };
            var customer = new { Name = "Jack" };

            Action act = () => subject.Should().BeEquivalentTo(customer, options => options.Including(ctx => ctx.Path == "Name"));

            act.Should().Throw<XunitException>().WithMessage("*Include member when*Name*");
        }

        [Fact]
        public void When_a_predicate_for_properties_to_include_has_been_specified_it_should_ignore_the_other_properties()
        {
            var subject = new { Age = 36, Birthdate = new DateTime(1973, 9, 20), Name = "John" };
            var customer = new { Age = 36, Birthdate = new DateTime(1973, 9, 20), Name = "Dennis" };

            subject.Should().BeEquivalentTo(customer, options => options
                .Including(info => info.Path.EndsWith("Age", StringComparison.Ordinal))
                .Including(info => info.Path.EndsWith("Birthdate", StringComparison.Ordinal)));
        }

        [Fact]
        public void When_a_non_property_expression_is_provided_it_should_throw()
        {
            var dto = new CustomerDto();

            Action act = () => dto.Should().BeEquivalentTo(dto, options => options.Including(d => d.GetType()));

            act.Should().Throw<ArgumentException>()
                .WithMessage("Expression <d.GetType()> cannot be used to select a member.*")
                .WithParameterName("expression");
        }

        [Fact]
        public void When_including_a_property_it_should_exactly_match_the_property()
        {
            var actual = new { DeclaredType = LocalOtherType.NonDefault, Type = LocalType.NonDefault };
            var expectation = new { DeclaredType = LocalOtherType.NonDefault };

            actual.Should().BeEquivalentTo(expectation, config => config.Including(o => o.DeclaredType));
        }

        private enum LocalOtherType : byte
        {
            Default,
            NonDefault
        }

        private enum LocalType : byte
        {
            Default,
            NonDefault
        }

        public class CustomType
        {
            [UsedImplicitly]
            public string Name { get; set; }
        }

        [Fact]
        public void When_including_a_property_using_an_expression_it_should_evaluate_it_from_the_root()
        {
            var list1 = new List<CustomType> { new() { Name = "A" }, new() { Name = "B" } };
            var subject = new ClassA { ListOfCustomTypes = list1 };
            var list2 = new List<CustomType> { new() { Name = "C" }, new() { Name = "D" } };
            var expectation = new ClassA { ListOfCustomTypes = list2 };

            Action act = () =>
                subject.Should().BeEquivalentTo(expectation, options => options.Including(x => x.ListOfCustomTypes));

            act.Should().Throw<XunitException>().WithMessage("*C*but*A*D*but*B*");
        }

        private class ClassA
        {
            public List<CustomType> ListOfCustomTypes { get; set; }
        }

        [Fact]
        public void When_null_is_provided_as_property_expression_it_should_throw()
        {
            var dto = new CustomerDto();

            Action act = () => dto.Should().BeEquivalentTo(dto, options => options.Including(null));

            act.Should().Throw<ArgumentNullException>().WithMessage("Expected an expression, but found <null>.*");
        }

        [Fact]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void When_including_fields_it_should_succeed_if_just_the_included_field_match()
        {
            var class1 = new ClassWithSomeFieldsAndProperties
            {
                Field1 = "Lorem", Field2 = "ipsum", Field3 = "dolor",
                Property1 = "sit", Property2 = "amet", Property3 = "consectetur"
            };
            var class2 = new ClassWithSomeFieldsAndProperties
            {
                Field1 = "Lorem", Field2 = "ipsum"
            };

            Action act = () =>
                class1.Should().BeEquivalentTo(class2, opts => opts.Including(o => o.Field1).Including(o => o.Field2));

            act.Should().NotThrow("the only selected fields have the same value");
        }

        [Fact]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void When_including_fields_it_should_fail_if_any_included_field_do_not_match()
        {
            var class1 = new ClassWithSomeFieldsAndProperties
            {
                Field1 = "Lorem", Field2 = "ipsum", Field3 = "dolor",
                Property1 = "sit", Property2 = "amet", Property3 = "consectetur"
            };
            var class2 = new ClassWithSomeFieldsAndProperties
            {
                Field1 = "Lorem", Field2 = "ipsum"
            };

            Action act = () => class1.Should().BeEquivalentTo(class2,
                opts => opts.Including(o => o.Field1).Including(o => o.Field2).Including(o => o.Field3));

            act.Should().Throw<XunitException>().WithMessage("Expected field class1.Field3*");
        }

        [Fact]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void When_both_field_and_properties_are_configured_for_inclusion_both_should_be_included()
        {
            var class1 = new ClassWithSomeFieldsAndProperties { Field1 = "Lorem", Property1 = "sit" };
            var class2 = new ClassWithSomeFieldsAndProperties();

            Action act =
                () => class1.Should().BeEquivalentTo(class2, opts => opts.IncludingFields().IncludingProperties());

            act.Should().Throw<XunitException>().Which.Message.Should().Contain("Field1").And.Contain("Property1");
        }

        [Fact]
        public void Including_nested_objects_restores_the_default_behavior()
        {
            var subject = new Root
            {
                Text = "Root",
                Level = new Level1 { Text = "Level1", Level = new Level2 { Text = "Mismatch" } }
            };
            var expected = new RootDto
            {
                Text = "Root",
                Level = new Level1Dto { Text = "Level1", Level = new Level2Dto { Text = "Level2" } }
            };

            Action act = () =>
                subject.Should().BeEquivalentTo(expected, options => options.WithoutRecursing().IncludingNestedObjects());

            act.Should().Throw<XunitException>().WithMessage("*Level.Level.Text*Mismatch*Level2*");
        }

#if NETCOREAPP3_0_OR_GREATER
        [Fact]
        public void Can_include_a_default_interface_property_using_an_expression()
        {
            IHaveDefaultProperty subject = new ClassReceivedDefaultInterfaceProperty { NormalProperty = "Value" };
            IHaveDefaultProperty expectation = new ClassReceivedDefaultInterfaceProperty { NormalProperty = "Another Value" };

            var act = () => subject.Should().BeEquivalentTo(expectation, x => x.Including(p => p.DefaultProperty));

            act.Should().Throw<XunitException>().WithMessage("Expected property subject.DefaultProperty to be 13, but found 5.*");
        }

        [Fact]
        public void Can_include_a_default_interface_property_using_a_name()
        {
            IHaveDefaultProperty subject = new ClassReceivedDefaultInterfaceProperty { NormalProperty = "Value" };
            IHaveDefaultProperty expectation = new ClassReceivedDefaultInterfaceProperty { NormalProperty = "Another Value" };

            var act = () => subject.Should().BeEquivalentTo(expectation,
                x => x.Including(p => p.Name.Contains("DefaultProperty")));

            act.Should().Throw<XunitException>().WithMessage("Expected property subject.DefaultProperty to be 13, but found 5.*");
        }

        private class ClassReceivedDefaultInterfaceProperty : IHaveDefaultProperty
        {
            public string NormalProperty { get; set; }
        }

        private interface IHaveDefaultProperty
        {
            string NormalProperty { get; set; }

            int DefaultProperty => NormalProperty.Length;
        }
#endif

        [Fact]
        public void An_anonymous_object_selects_correctly()
        {
            var subject = new ClassWithSomeFieldsAndProperties
            {
                Field1 = "Lorem", Field2 = "ipsum", Field3 = "dolor",
                Property1 = "sit", Property2 = "amet", Property3 = "consectetur"
            };
            var expectation = new ClassWithSomeFieldsAndProperties
            {
                Field1 = "Lorem", Field2 = "ipsum"
            };

            Action act = () => subject.Should().BeEquivalentTo(expectation,
                opts => opts.Including(o => new { o.Field1, o.Field2, o.Field3 }));

            act.Should().Throw<XunitException>().WithMessage("Expected field subject.Field3*");
        }

        [Fact]
        public void An_anonymous_object_selects_nested_fields_correctly()
        {
            var subject = new { Field = "Lorem", NestedField = new { FieldB = "ipsum" } };
            var expectation = new { FieldA = "Lorem", NestedField = new { FieldB = "no ipsum" } };

            Action act = () => subject.Should().BeEquivalentTo(expectation,
                opts => opts.Including(o => new { o.NestedField.FieldB }));

            act.Should().Throw<XunitException>().WithMessage("Expected*subject.NestedField.FieldB*");
        }

        [Fact]
        public void An_anonymous_object_in_combination_with_exclude_selects_nested_fields_correctly()
        {
            var subject = new
            {
                FieldA = "Lorem",
                NestedField = new { FieldB = "ipsum", FieldC = "ipsum2", FieldD = "ipsum3", FieldE = "ipsum4" }
            };
            var expectation = new
            {
                FieldA = "Lorem",
                NestedField = new { FieldB = "no ipsum", FieldC = "no ipsum2", FieldD = "no ipsum3", FieldE = "no ipsum4" }
            };

            Action act = () => subject.Should().BeEquivalentTo(expectation, opts => opts
                .Excluding(o => new { o.NestedField })
                .Including(o => new { o.NestedField.FieldB }));

            act.Should().Throw<XunitException>()
                .Which.Message.Should()
                .Match("*Expected*subject.NestedField.FieldB*").And
                .NotMatch("*Expected*FieldC*FieldD*FieldE*");
        }
    }
}
