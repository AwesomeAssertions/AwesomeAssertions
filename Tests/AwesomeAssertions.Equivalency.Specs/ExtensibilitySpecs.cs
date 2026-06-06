using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using AwesomeAssertions.Execution;
using AwesomeAssertions.Extensions;
using JetBrains.Annotations;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Equivalency.Specs;

/// <summary>
/// Test Class containing specs over the extensibility points of Should().BeEquivalentTo
/// </summary>
public static class ExtensibilitySpecs
{
    public class SelectionRulesSpecs
    {
        [Fact]
        public void When_a_selection_rule_is_added_it_should_be_evaluated_after_all_existing_rules()
        {
            var subject = new { NameId = "123", SomeValue = "hello" };
            var expected = new { SomeValue = "hello" };

            subject.Should().BeEquivalentTo(
                expected,
                options => options.Using(new ExcludeForeignKeysSelectionRule()));
        }

        [Fact]
        public void When_a_selection_rule_is_added_it_should_appear_in_the_exception_message()
        {
            var subject = new { Name = "123" };
            var expected = new { SomeValue = "hello" };

            Action act = () =>
                subject.Should().BeEquivalentTo(expected, options => options.Using(new ExcludeForeignKeysSelectionRule()));

            act.Should().Throw<XunitException>()
                .WithMessage($"*{nameof(ExcludeForeignKeysSelectionRule)}*");
        }

        private class ExcludeForeignKeysSelectionRule : IMemberSelectionRule
        {
            public IEnumerable<IMember> SelectMembers(INode currentNode, IEnumerable<IMember> selectedMembers,
                MemberSelectionContext context)
            {
                return selectedMembers.Where(pi => !pi.Subject.Name.EndsWith("Id", StringComparison.Ordinal)).ToArray();
            }

            bool IMemberSelectionRule.IncludesMembers
            {
                get { return false; }
            }
        }
    }

    public class MatchingRulesSpecs
    {
        [Fact]
        public void When_a_matching_rule_is_added_it_should_precede_all_existing_rules()
        {
            var subject = new { Name = "123", SomeValue = "hello" };
            var expected = new { NameId = "123", SomeValue = "hello" };

            subject.Should().BeEquivalentTo(
                expected,
                options => options.Using(new ForeignKeyMatchingRule()));
        }

        [Fact]
        public void When_a_matching_rule_is_added_it_should_appear_in_the_exception_message()
        {
            var subject = new { NameId = "123", SomeValue = "hello" };
            var expected = new { Name = "1234", SomeValue = "hello" };

            Action act = () => subject.Should().BeEquivalentTo(
                expected,
                options => options.Using(new ForeignKeyMatchingRule()));

            act.Should().Throw<XunitException>()
                .WithMessage($"*{nameof(ForeignKeyMatchingRule)}*");
        }

        private class ForeignKeyMatchingRule : IMemberMatchingRule
        {
            public IMember Match(IMember expectedMember, object subject, INode parent, IEquivalencyOptions options,
                AssertionChain assertionChain)
            {
                string name = expectedMember.Subject.Name;

                if (name.EndsWith("Id", StringComparison.Ordinal))
                {
                    name = name.Replace("Id", "");
                }

                PropertyInfo runtimeProperty = subject.GetType().GetRuntimeProperty(name);
                return runtimeProperty is not null ? new Property(runtimeProperty, parent) : null;
            }
        }
    }

    public class OrderingRulesSpecs
    {
        [Fact]
        public void When_an_ordering_rule_is_added_it_should_be_evaluated_after_all_existing_rules()
        {
            string[] subject = ["First", "Second"];
            string[] expected = ["First", "Second"];

            subject.Should().BeEquivalentTo(
                expected,
                options => options.Using(new StrictOrderingRule()));
        }

        [Fact]
        public void When_an_ordering_rule_is_added_it_should_appear_in_the_exception_message()
        {
            string[] subject = ["First", "Second"];
            string[] expected = ["Second", "First"];

            Action act = () => subject.Should().BeEquivalentTo(
                expected,
                options => options.Using(new StrictOrderingRule()));

            act.Should().Throw<XunitException>()
                .WithMessage($"*{nameof(StrictOrderingRule)}*");
        }

        private class StrictOrderingRule : IOrderingRule
        {
            public OrderStrictness Evaluate(IObjectInfo objectInfo)
            {
                return OrderStrictness.Strict;
            }
        }
    }

    public class AssertionRulesSpecs
    {
        [Fact]
        public void When_property_of_other_is_incompatible_with_generic_type_the_message_should_include_generic_type()
        {
            var subject = new { Id = "foo" };
            var other = new { Id = 0.5d };

            Action act = () => subject.Should().BeEquivalentTo(other,
                o => o
                    .Using<string>(c => c.Subject.Should().Be(c.Expectation))
                    .When(si => si.Path == "Id"));

            act.Should().Throw<XunitException>()
                .WithMessage("*Id*from expectation*string*double*");
        }

        [Fact]
        public void Can_exclude_all_properties_of_the_parent_type()
        {
            var subject = new { Id = "foo" };
            var expectation = new { Id = "bar" };

            subject.Should().BeEquivalentTo(expectation,
                o => o
                    .Using<string>(c => c.Subject.Should().HaveLength(c.Expectation.Length))
                    .When(si => si.ParentType == expectation.GetType() && si.Path.EndsWith("Id", StringComparison.Ordinal)));
        }

        [Fact]
        public void When_property_of_subject_is_incompatible_with_generic_type_the_message_should_include_generic_type()
        {
            var subject = new { Id = 0.5d };
            var other = new { Id = "foo" };

            Action act = () => subject.Should().BeEquivalentTo(other,
                o => o
                    .Using<string>(c => c.Subject.Should().Be(c.Expectation))
                    .When(si => si.Path == "Id"));

            act.Should().Throw<XunitException>()
                .WithMessage("*Id*from subject*string*double*");
        }

        [Fact]
        public void
            When_equally_named_properties_are_both_incompatible_with_generic_type_the_message_should_include_generic_type()
        {
            var subject = new { Id = 0.5d };
            var other = new { Id = 0.5d };

            Action act = () => subject.Should().BeEquivalentTo(other,
                o => o
                    .Using<string>(c => c.Subject.Should().Be(c.Expectation))
                    .When(si => si.Path == "Id"));

            act.Should().Throw<XunitException>()
                .WithMessage("*Id*from subject*string*double*");
        }

        [Fact]
        public void When_property_of_other_is_null_the_failure_message_should_not_complain_about_its_type()
        {
            var subject = new { Id = "foo" };
            var other = new { Id = null as double? };

            Action act = () => subject.Should().BeEquivalentTo(other,
                o => o
                    .Using<string>(c => c.Subject.Should().Be(c.Expectation))
                    .When(si => si.Path == "Id"));

            act.Should().Throw<XunitException>()
                .Which.Message.Should()
                .Contain("Expected property subject.Id to be <null>, but found \"foo\"")
                .And.NotContain("from expectation");
        }

        [Fact]
        public void When_property_of_subject_is_null_the_failure_message_should_not_complain_about_its_type()
        {
            var subject = new { Id = null as double? };
            var expectation = new { Id = "bar" };

            Action act = () => subject.Should().BeEquivalentTo(expectation,
                o => o
                    .Using<string>(c => c.Subject.Should().Be(c.Expectation))
                    .When(si => si.Path == "Id"));

            act.Should().Throw<XunitException>()
                .Which.Message.Should()
                .Contain("Expected property subject.Id to be \"bar\", but found <null>")
                .And.NotContain("from subject");
        }

        [Fact]
        public void When_equally_named_properties_are_both_null_it_should_succeed()
        {
            var subject = new { Id = null as double? };
            var other = new { Id = null as string };

            subject.Should().BeEquivalentTo(other,
                o => o
                    .Using<string>(c => c.Subject.Should().Be(c.Expectation))
                    .When(si => si.Path == "Id"));
        }

        [Fact]
        public void When_equally_named_properties_are_type_incompatible_and_assertion_rule_exists_it_should_not_throw()
        {
            var subject = new { Type = typeof(string) };
            var other = new { Type = typeof(string).AssemblyQualifiedName };

            subject.Should().BeEquivalentTo(other,
                o => o
                    .Using<object>(c => ((Type)c.Subject).AssemblyQualifiedName.Should().Be((string)c.Expectation))
                    .When(si => si.Path == "Type"));
        }

        [Fact]
        public void When_an_assertion_is_overridden_for_a_predicate_it_should_use_the_provided_action()
        {
            var subject = new { Date = 14.July(2012).At(12, 59, 59) };
            var expectation = new { Date = 14.July(2012).At(13, 0) };

            subject.Should().BeEquivalentTo(expectation, options => options
                .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 1.Seconds()))
                .When(info => info.Path.EndsWith("Date", StringComparison.Ordinal)));
        }

        [Fact]
        public void When_an_assertion_is_overridden_for_all_types_it_should_use_the_provided_action_for_all_properties()
        {
            var subject = new
            {
                Date = 21.July(2012).At(11, 8, 59),
                Nested = new
                {
                    NestedDate = 14.July(2012).At(12, 59, 59)
                }
            };
            var expectation = new
            {
                Date = 21.July(2012).At(11, 9),
                Nested = new
                {
                    NestedDate = 14.July(2012).At(13, 0)
                }
            };

            subject.Should().BeEquivalentTo(expectation, options =>
                options.Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 1.Seconds()))
                    .WhenTypeIs<DateTime>());
        }

        [InlineData(null, 0)]
        [InlineData(0, null)]
        [Theory]
        public void When_subject_or_expectation_is_null_it_should_not_match_a_non_nullable_type(
            int? subjectValue, int? expectedValue)
        {
            var actual = new { Value = subjectValue };
            var expected = new { Value = expectedValue };

            Action act = () => actual.Should().BeEquivalentTo(expected, opt => opt
                .Using<int>(c => c.Subject.Should().NotBe(c.Expectation))
                .WhenTypeIs<int>());

            act.Should().Throw<XunitException>();
        }

        [InlineData(null, 0)]
        [InlineData(0, null)]
        [Theory]
        public void When_subject_or_expectation_is_null_it_should_match_a_nullable_type(int? subjectValue, int? expectedValue)
        {
            var actual = new { Value = subjectValue };
            var expected = new { Value = expectedValue };

            actual.Should().BeEquivalentTo(expected, opt => opt
                .Using<int?>(c => c.Subject.Should().NotBe(c.Expectation))
                .WhenTypeIs<int?>());
        }

        [InlineData(null, null)]
        [InlineData(0, 0)]
        [Theory]
        public void When_types_are_nullable_it_should_match_a_nullable_type(int? subjectValue, int? expectedValue)
        {
            var actual = new { Value = subjectValue };
            var expected = new { Value = expectedValue };

            Action act = () => actual.Should().BeEquivalentTo(expected, opt => opt
                .Using<int?>(c => c.Subject.Should().NotBe(c.Expectation))
                .WhenTypeIs<int?>());

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_overriding_with_custom_assertion_it_should_be_chainable()
        {
            var actual = new { Nullable = (int?)1, NonNullable = 2 };
            var expected = new { Nullable = (int?)3, NonNullable = 3 };

            actual.Should().BeEquivalentTo(expected, opt => opt
                .Using<int>(c => c.Subject.Should().BeCloseTo(c.Expectation, 1))
                .WhenTypeIs<int>()
                .Using<int?>(c => c.Subject.Should().NotBe(c.Expectation))
                .WhenTypeIs<int?>());
        }

        [Fact]
        public void When_a_nullable_property_is_overridden_with_a_custom_assertion_it_should_use_it()
        {
            var actual = new SimpleWithNullable
            {
                NullableIntegerProperty = 1,
                StringProperty = "I haz a string!"
            };
            var expected = new SimpleWithNullable
            {
                StringProperty = "I haz a string!"
            };

            actual.Should().BeEquivalentTo(expected, opt => opt
                .Using<long?>(c => c.Subject.Should().BeInRange(0, 10))
                .WhenTypeIs<long?>());
        }

        private class SimpleWithNullable
        {
            public long? NullableIntegerProperty { get; set; }

            public string StringProperty { get; set; }
        }

        [Fact]
        public void When_an_assertion_rule_is_added_it_should_precede_all_existing_rules()
        {
            var subject = new { Created = 8.July(2012).At(22, 9) };
            var expected = new { Created = 8.July(2012).At(22, 10) };

            subject.Should().BeEquivalentTo(
                expected,
                options => options.Using(new RelaxingDateTimeEquivalencyStep()));
        }

        [Fact]
        public void When_an_assertion_rule_is_added_it_appear_in_the_exception_message()
        {
            var subject = new { Property = 8.July(2012).At(22, 9) };
            var expected = new { Property = 8.July(2012).At(22, 11) };

            Action act = () => subject.Should().BeEquivalentTo(
                expected,
                options => options.Using(new RelaxingDateTimeEquivalencyStep()));

            act.Should().Throw<XunitException>()
                .WithMessage($"*{nameof(RelaxingDateTimeEquivalencyStep)}*");
        }

        [Fact]
        public void When_multiple_steps_are_added_they_should_be_evaluated_first_to_last()
        {
            var subject = new { Created = 8.July(2012).At(22, 9) };
            var expected = new { Created = 8.July(2012).At(22, 10) };

            Action act = () => subject.Should().BeEquivalentTo(expected, opts => opts
                .Using(new RelaxingDateTimeEquivalencyStep())
                .Using(new AlwaysFailOnDateTimesEquivalencyStep()));

            act.Should().NotThrow(
                "a different assertion rule should handle the comparison before the exception throwing assertion rule is hit");
        }

        private class AlwaysFailOnDateTimesEquivalencyStep : IEquivalencyStep
        {
            public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context,
                IValidateChildNodeEquivalency valueChildNodes)
            {
                if (comparands.Expectation is DateTime)
                {
                    throw new Exception("Failed");
                }

                return EquivalencyResult.ContinueWithNext;
            }
        }

        private class RelaxingDateTimeEquivalencyStep : IEquivalencyStep
        {
            public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context,
                IValidateChildNodeEquivalency valueChildNodes)
            {
                if (comparands.Expectation is DateTime time)
                {
                    ((DateTime)comparands.Subject).Should().BeCloseTo(time, 1.Minutes());

                    return EquivalencyResult.EquivalencyProven;
                }

                return EquivalencyResult.ContinueWithNext;
            }
        }

        [Fact]
        public void When_multiple_assertion_rules_are_added_with_the_fluent_api_they_should_be_executed_from_right_to_left()
        {
            var subject = new ClassWithOnlyAProperty();
            var expected = new ClassWithOnlyAProperty();

            Action act =
                () => subject.Should().BeEquivalentTo(expected, opts =>
                    opts.Using<object>(_ => throw new Exception())
                        .When(_ => true)
                        .Using<object>(_ => { })
                        .When(_ => true));

            act.Should().NotThrow(
                "a different assertion rule should handle the comparison before the exception throwing assertion rule is hit");
        }

        [Fact]
        public void When_using_a_nested_equivalency_api_in_a_custom_assertion_rule_it_should_honor_the_rule()
        {
            var subject = new ClassWithSomeFieldsAndProperties
            {
                Property1 = "value1",
                Property2 = "value2"
            };
            var expectation = new ClassWithSomeFieldsAndProperties
            {
                Property1 = "value1",
                Property2 = "value3"
            };

            subject.Should().BeEquivalentTo(expectation, options => options
                .Using<ClassWithSomeFieldsAndProperties>(ctx =>
                    ctx.Subject.Should()
                        .BeEquivalentTo(ctx.Expectation, nestedOptions => nestedOptions.Excluding(x => x.Property2)))
                .WhenTypeIs<ClassWithSomeFieldsAndProperties>());
        }

        [Fact]
        public void When_a_predicate_matches_after_auto_conversion_it_should_execute_the_assertion()
        {
            var expectation = new { ThisIsMyDateTime = DateTime.Now };
            var actual = new
            {
                ThisIsMyDateTime = expectation.ThisIsMyDateTime.ToString(CultureInfo.InvariantCulture)
            };

            actual.Should().BeEquivalentTo(expectation,
                options => options
                    .WithAutoConversion()
                    .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 1.Seconds()))
                    .WhenTypeIs<DateTime>());
        }
    }

    public class EquivalencyStepsSpecs
    {
        [Fact]
        public void When_an_equivalency_step_handles_the_comparison_later_equivalency_steps_should_not_be_ran()
        {
            var subject = new ClassWithOnlyAProperty();
            var expected = new ClassWithOnlyAProperty();

            subject.Should().BeEquivalentTo(expected,
                opts =>
                    opts.Using(new AlwaysHandleEquivalencyStep())
                        .Using(new ThrowExceptionEquivalencyStep<InvalidOperationException>()));
        }

        [Fact]
        public void When_a_user_equivalency_step_is_registered_it_should_run_before_the_built_in_steps()
        {
            var actual = new { Property = 123 };
            var expected = new { Property = "123" };

            Action act = () => actual.Should().BeEquivalentTo(expected, options => options
                .Using(new EqualityEquivalencyStep()));

            act.Should().Throw<XunitException>()
                .WithMessage("Expected*123*123*");
        }

        [Fact]
        public void When_an_equivalency_does_not_handle_the_comparison_later_equivalency_steps_should_still_be_ran()
        {
            var subject = new ClassWithOnlyAProperty();
            var expected = new ClassWithOnlyAProperty();

            Action act = () =>
                subject.Should().BeEquivalentTo(expected, opts =>
                    opts.Using(new NeverHandleEquivalencyStep())
                        .Using(new ThrowExceptionEquivalencyStep<InvalidOperationException>()));

            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void When_multiple_equivalency_steps_are_added_they_should_be_executed_in_registration_order()
        {
            var subject = new ClassWithOnlyAProperty();
            var expected = new ClassWithOnlyAProperty();

            Action act = () =>
                subject.Should().BeEquivalentTo(expected, opts =>
                    opts.Using(new ThrowExceptionEquivalencyStep<NotSupportedException>())
                        .Using(new ThrowExceptionEquivalencyStep<InvalidOperationException>()));

            act.Should().Throw<NotSupportedException>();
        }

        private class ThrowExceptionEquivalencyStep<TException> : IEquivalencyStep
            where TException : Exception, new()
        {
            public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context,
                IValidateChildNodeEquivalency valueChildNodes)
            {
                throw new TException();
            }
        }

        private class AlwaysHandleEquivalencyStep : IEquivalencyStep
        {
            public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context,
                IValidateChildNodeEquivalency valueChildNodes)
            {
                return EquivalencyResult.EquivalencyProven;
            }
        }

        private class NeverHandleEquivalencyStep : IEquivalencyStep
        {
            public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context,
                IValidateChildNodeEquivalency valueChildNodes)
            {
                return EquivalencyResult.ContinueWithNext;
            }
        }

        private class EqualityEquivalencyStep : IEquivalencyStep
        {
            public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context,
                IValidateChildNodeEquivalency valueChildNodes)
            {
                comparands.Subject.Should().Be(comparands.Expectation, context.Reason.FormattedMessage, context.Reason.Arguments);
                return EquivalencyResult.EquivalencyProven;
            }
        }

        [Fact]
        public void Can_compare_null_against_null_with_custom_comparer_for_nullable_property()
        {
            var subject = new ClassWithNullableStructProperty();

            subject.Should().BeEquivalentTo(new ClassWithNullableStructProperty(), o => o
                .Using<StructWithProperties, StructWithPropertiesComparer>());
        }

        [Fact]
        public void Can_compare_null_against_not_null_with_custom_comparer_for_nullable_property()
        {
            var subject = new ClassWithNullableStructProperty();
            var unexpected = new ClassWithNullableStructProperty { Value = new StructWithProperties { Value = 42 } };

            subject.Should().NotBeEquivalentTo(unexpected, o => o
                .Using<StructWithProperties, StructWithPropertiesComparer>());
        }

        [Fact]
        public void Can_compare_not_null_against_null_with_custom_comparer_for_nullable_property()
        {
            var subject = new ClassWithNullableStructProperty
            {
                Value = new StructWithProperties { Value = 42 },
            };

            subject.Should().NotBeEquivalentTo(new ClassWithNullableStructProperty(), o => o
                .Using<StructWithProperties, StructWithPropertiesComparer>());
        }

        [Fact]
        public void Can_compare_not_null_against_not_null_with_custom_comparer_for_nullable_property()
        {
            var subject = new ClassWithNullableStructProperty { Value = new StructWithProperties { Value = 42 } };
            var expected = new ClassWithNullableStructProperty { Value = new StructWithProperties { Value = 42 } };

            subject.Should().BeEquivalentTo(expected, o => o
                .Using<StructWithProperties, StructWithPropertiesComparer>());
        }

        [Fact]
        public void Can_compare_null_against_null_with_custom_nullable_comparer_for_nullable_property()
        {
            var subject = new ClassWithNullableStructProperty();

            subject.Should().BeEquivalentTo(new ClassWithNullableStructProperty(), o => o
                .Using<StructWithProperties?, StructWithPropertiesComparer>());
        }

        [Fact]
        public void Can_compare_null_against_not_null_with_custom_nullable_comparer_for_nullable_property()
        {
            var subject = new ClassWithNullableStructProperty();
            var unexpected = new ClassWithNullableStructProperty { Value = new StructWithProperties { Value = 42 } };

            subject.Should().NotBeEquivalentTo(unexpected, o => o
                .Using<StructWithProperties?, StructWithPropertiesComparer>()
            );
        }

        [Fact]
        public void Can_compare_not_null_against_null_with_custom_nullable_comparer_for_nullable_property()
        {
            var subject = new ClassWithNullableStructProperty { Value = new StructWithProperties { Value = 42 } };

            subject.Should().NotBeEquivalentTo(new ClassWithNullableStructProperty(), o => o
                .Using<StructWithProperties?, StructWithPropertiesComparer>()
            );
        }

        [Fact]
        public void Can_compare_not_null_against_not_null_with_custom_nullable_comparer_for_nullable_property()
        {
            var subject = new ClassWithNullableStructProperty { Value = new StructWithProperties { Value = 42 } };
            var expected = new ClassWithNullableStructProperty { Value = new StructWithProperties { Value = 42 } };

            subject.Should().BeEquivalentTo(expected, o => o
                .Using<StructWithProperties?, StructWithPropertiesComparer>());
        }

        private class ClassWithNullableStructProperty
        {
            [UsedImplicitly]
            public StructWithProperties? Value { get; set; }
        }

        private struct StructWithProperties
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public int Value { get; set; }
        }

        private class StructWithPropertiesComparer
            : IEqualityComparer<StructWithProperties>, IEqualityComparer<StructWithProperties?>
        {
            public bool Equals(StructWithProperties x, StructWithProperties y) => Equals(x.Value, y.Value);

            public int GetHashCode(StructWithProperties obj) => obj.Value;

            public bool Equals(StructWithProperties? x, StructWithProperties? y) => Equals(x?.Value, y?.Value);

            public int GetHashCode(StructWithProperties? obj) => obj?.Value ?? 0;
        }

        [Fact]
        public void Can_compare_null_against_null_with_custom_comparer_for_property()
        {
            var subject = new ClassWithClassProperty();

            subject.Should().BeEquivalentTo(new ClassWithClassProperty(), o => o
                .Using<ClassProperty, ClassPropertyComparer>());
        }

        [Fact]
        public void Can_compare_null_against_not_null_with_custom_comparer_for_property()
        {
            var subject = new ClassWithClassProperty();
            var unexpected = new ClassWithClassProperty { Value = new ClassProperty { Value = 42 } };

            subject.Should().NotBeEquivalentTo(unexpected, o => o
                .Using<ClassProperty, ClassPropertyComparer>());
        }

        [Fact]
        public void Can_compare_not_null_against_null_with_custom_comparer_for_property()
        {
            var subject = new ClassWithClassProperty { Value = new ClassProperty { Value = 42 } };

            subject.Should().NotBeEquivalentTo(new ClassWithClassProperty(), o => o
                .Using<ClassProperty, ClassPropertyComparer>());
        }

        [Fact]
        public void Can_compare_not_null_against_not_null_with_custom_comparer_for_property()
        {
            var subject = new ClassWithClassProperty { Value = new ClassProperty { Value = 42 } };
            var expected = new ClassWithClassProperty { Value = new ClassProperty { Value = 42 } };

            subject.Should().BeEquivalentTo(expected, o => o
                .Using<ClassProperty, ClassPropertyComparer>());
        }

        private class ClassWithClassProperty
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public ClassProperty Value { get; set; }
        }

        private class ClassProperty
        {
            public int Value { get; set; }
        }

        private class ClassPropertyComparer : IEqualityComparer<ClassProperty>
        {
            public bool Equals(ClassProperty x, ClassProperty y) => Equals(x?.Value, y?.Value);

            public int GetHashCode(ClassProperty obj) => obj.Value;
        }
    }
}
