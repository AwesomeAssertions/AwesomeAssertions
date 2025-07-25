using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AwesomeAssertions.Extensions;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Equivalency.Specs;

public class AssertionRuleSpecs
{
    [Fact]
    public void When_two_objects_have_the_same_property_values_it_should_succeed()
    {
        // Arrange
        var subject = new { Age = 36, Birthdate = new DateTime(1973, 9, 20), Name = "Dennis" };

        var other = new { Age = 36, Birthdate = new DateTime(1973, 9, 20), Name = "Dennis" };

        // Act / Assert
        subject.Should().BeEquivalentTo(other);
    }

    [Fact]
    public void When_two_objects_have_the_same_nullable_property_values_it_should_succeed()
    {
        // Arrange
        var subject = new { Age = 36, Birthdate = (DateTime?)new DateTime(1973, 9, 20), Name = "Dennis" };

        var other = new { Age = 36, Birthdate = (DateTime?)new DateTime(1973, 9, 20), Name = "Dennis" };

        // Act / Assert
        subject.Should().BeEquivalentTo(other);
    }

    [Fact]
    public void When_two_objects_have_the_same_properties_but_a_different_value_it_should_throw()
    {
        // Arrange
        var subject = new { Age = 36 };

        var expectation = new { Age = 37 };

        // Act
        Action act = () => subject.Should().BeEquivalentTo(expectation, "because {0} are the same", "they");

        // Assert
        act.Should().Throw<XunitException>().WithMessage(
            "Expected*Age*to be 37 because they are the same, but found 36*");
    }

    [Fact]
    public void
        When_subject_has_a_valid_property_that_is_compared_with_a_null_property_it_should_throw_with_descriptive_message()
    {
        // Arrange
        var subject = new { Name = "Dennis" };

        var other = new { Name = (string)null };

        // Act
        Action act = () => subject.Should().BeEquivalentTo(other, "we want to test the failure {0}", "message");

        // Assert
        act.Should().Throw<XunitException>().WithMessage(
            "Expected property subject.Name to be <null>*we want to test the failure message*, but found \"Dennis\"*");
    }

    [Fact]
    public void When_two_collection_properties_dont_match_it_should_throw_and_specify_the_difference()
    {
        // Arrange
        var subject = new { Values = new[] { 1, 2, 3 } };

        var other = new { Values = new[] { 1, 4, 3 } };

        // Act
        Action act = () => subject.Should().BeEquivalentTo(other);

        // Assert
        act.Should().Throw<XunitException>().WithMessage(
            "Expected*Values[1]*to be 4, but found 2*");
    }

    [Fact]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void When_two_string_properties_do_not_match_it_should_throw_and_state_the_difference()
    {
        // Arrange
        var subject = new { Name = "Dennes" };

        var other = new { Name = "Dennis" };

        // Act
        Action act = () => subject.Should().BeEquivalentTo(other, options => options.Including(d => d.Name));

        // Assert
        act.Should().Throw<XunitException>().WithMessage(
            "Expected*Name to be*index 4*\"Dennes\"*\"Dennis\"*");
    }

    [Fact]
    public void When_two_properties_are_of_derived_types_but_are_equal_it_should_succeed()
    {
        // Arrange
        var subject = new { Type = new DerivedCustomerType("123") };

        var other = new { Type = new CustomerType("123") };

        // Act / Assert
        subject.Should().BeEquivalentTo(other);
    }

    [Fact]
    public void
        When_two_properties_have_the_same_declared_type_but_different_runtime_types_and_are_equivalent_according_to_the_declared_type_it_should_succeed()
    {
        // Arrange
        var subject = new { Type = (CustomerType)new DerivedCustomerType("123") };

        var other = new { Type = new CustomerType("123") };

        // Act / Assert
        subject.Should().BeEquivalentTo(other);
    }

    [Fact]
    public void When_a_nested_property_is_equal_based_on_equality_comparer_it_should_not_throw()
    {
        // Arrange
        var subject = new { Timestamp = 22.March(2020).At(19, 30) };

        var expectation = new { Timestamp = 1.January(2020).At(7, 31) };

        // Act / Assert
        subject.Should().BeEquivalentTo(expectation,
            opt => opt.Using<DateTime, DateTimeByYearComparer>());
    }

    [Fact]
    public void When_a_nested_property_is_unequal_based_on_equality_comparer_it_should_throw()
    {
        // Arrange
        var subject = new { Timestamp = 22.March(2020) };

        var expectation = new { Timestamp = 1.January(2021) };

        // Act
        Action act = () => subject.Should().BeEquivalentTo(expectation,
            opt => opt.Using(new DateTimeByYearComparer()));

        // Assert
        act.Should()
            .Throw<XunitException>()
            .WithMessage("Expected*equal*2021*DateTimeByYearComparer*2020*");
    }

    [Fact]
    public void When_the_subjects_property_type_is_different_from_the_equality_comparer_it_should_throw()
    {
        // Arrange
        var subject = new { Timestamp = 1L };

        var expectation = new { Timestamp = 1.January(2021) };

        // Act
        Action act = () => subject.Should().BeEquivalentTo(expectation,
            opt => opt.Using(new DateTimeByYearComparer()));

        // Assert
        act.Should()
            .Throw<XunitException>()
            .WithMessage("Expected*Timestamp*1L*");
    }

    private class DateTimeByYearComparer : IEqualityComparer<DateTime>
    {
        public bool Equals(DateTime x, DateTime y)
        {
            return x.Year == y.Year;
        }

        public int GetHashCode(DateTime obj) => obj.GetHashCode();
    }

    [Fact]
    public void When_an_invalid_equality_comparer_is_provided_it_should_throw()
    {
        // Arrange
        var subject = new { Timestamp = 22.March(2020) };

        var expectation = new { Timestamp = 1.January(2021) };

        IEqualityComparer<DateTime> equalityComparer = null;

        // Act
        Action act = () => subject.Should().BeEquivalentTo(expectation,
            opt => opt.Using(equalityComparer));

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("*comparer*");
    }

    [Fact]
    public void When_the_compile_time_type_does_not_match_the_equality_comparer_type_it_should_use_the_default_mechanics()
    {
        // Arrange
        var subject = new { Property = (IInterface)new ConcreteClass("SomeString") };

        var expectation = new { Property = (IInterface)new ConcreteClass("SomeOtherString") };

        // Act / Assert
        subject.Should().BeEquivalentTo(expectation, opt =>
            opt.Using<ConcreteClass, ConcreteClassEqualityComparer>());
    }

    [Fact]
    public void When_the_runtime_type_does_match_the_equality_comparer_type_it_should_use_the_default_mechanics()
    {
        // Arrange
        var subject = new { Property = (IInterface)new ConcreteClass("SomeString") };

        var expectation = new { Property = (IInterface)new ConcreteClass("SomeOtherString") };

        // Act
        Action act = () => subject.Should().BeEquivalentTo(expectation, opt => opt
            .PreferringRuntimeMemberTypes()
            .Using<ConcreteClass, ConcreteClassEqualityComparer>());

        // Assert
        act.Should().Throw<XunitException>().WithMessage("*ConcreteClassEqualityComparer*");
    }

    [Fact]
    public void When_several_properties_do_not_match_they_are_all_reported_after_an_other_successful_assertion()
    {
        // Arrange
        var test = new Test
        {
            Id = 42,
            Text = "some-text",
        };
        var actualTest = new Test
        {
            Id = 32,
            Text = "some-other-text",
        };

        // Act
        Action act = () =>
        {
            actualTest.Text.Should().Be("some-other-text");
            actualTest.Should().BeEquivalentTo(test);
        };

        // Assert
        act.Should().Throw<XunitException>()
            .WithMessage(
                $"Expected property actualTest.Id to be*{Environment.NewLine}" +
                "*Expected property actualTest.Text to be*"
            );
    }

    private sealed class Test
    {
        public int Id { get; set; }

        public string Text { get; set; }
    }

    public class BeEquivalentTo
    {
        [Fact]
        public void An_equality_comparer_of_non_nullable_type_is_not_invoked_on_nullable_member()
        {
            // Arrange
            var subject = new { Timestamp = (DateTime?)22.March(2020) };

            var expectation = new { Timestamp = (DateTime?)1.January(2020) };

            // Act
            Action act = () => subject.Should().BeEquivalentTo(expectation,
                opt => opt.Using(new DateTimeByYearComparer()));

            // Assert
            act.Should().Throw<XunitException>().WithMessage("Expected*Timestamp*2020*");
        }

        [Fact]
        public void An_equality_comparer_of_nullable_type_is_not_invoked_on_non_nullable_member()
        {
            // Arrange
            var subject = new { Timestamp = 22.March(2020) };

            var expectation = new { Timestamp = 1.January(2020) };

            // Act
            Action act = () => subject.Should().BeEquivalentTo(expectation,
                opt => opt.Using(new NullableDateTimeByYearComparer()));

            // Assert
            act.Should().Throw<XunitException>().WithMessage("Expected*Timestamp*2020*");
        }

        [Fact]
        public void An_equality_comparer_of_non_nullable_type_is_invoked_on_non_nullable_data()
        {
            // Arrange
            var subject = new { Timestamp = (DateTime?)22.March(2020) };

            var expectation = new { Timestamp = (DateTime?)1.January(2020) };

            // Act
            subject.Should().BeEquivalentTo(expectation, opt => opt
                .PreferringRuntimeMemberTypes()
                .Using(new DateTimeByYearComparer()));
        }

        [Fact]
        public void An_equality_comparer_of_nullable_type_is_not_invoked_on_non_nullable_data()
        {
            // Arrange
            var subject = new { Timestamp = (DateTime?)22.March(2020) };

            var expectation = new { Timestamp = (DateTime?)1.January(2020) };

            // Act
            Action act = () => subject.Should().BeEquivalentTo(expectation, opt => opt
                .PreferringRuntimeMemberTypes()
                .Using(new NullableDateTimeByYearComparer()));

            // Assert
            act.Should().Throw<XunitException>().WithMessage("Expected*Timestamp*2020*");
        }
    }

    private interface IInterface;

    private class ConcreteClass : IInterface
    {
        private readonly string property;

        public ConcreteClass(string propertyValue)
        {
            property = propertyValue;
        }

        public string GetProperty() => property;
    }

    private class ConcreteClassEqualityComparer : IEqualityComparer<ConcreteClass>
    {
        public bool Equals(ConcreteClass x, ConcreteClass y)
        {
            return x.GetProperty() == y.GetProperty();
        }

        public int GetHashCode(ConcreteClass obj) => obj.GetProperty().GetHashCode();
    }

    private class NullableDateTimeByYearComparer : IEqualityComparer<DateTime?>
    {
        public bool Equals(DateTime? x, DateTime? y)
        {
            return x?.Year == y?.Year;
        }

        public int GetHashCode(DateTime? obj) => obj?.GetHashCode() ?? 0;
    }
}
