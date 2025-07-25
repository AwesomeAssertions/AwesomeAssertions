using System;
using System.ComponentModel;
using JetBrains.Annotations;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Equivalency.Specs;

public partial class SelectionRulesSpecs
{
    public class Browsability
    {
        [Fact]
        public void When_browsable_field_differs_excluding_non_browsable_members_should_not_affect_result()
        {
            // Arrange
            var subject = new ClassWithNonBrowsableMembers
            {
                BrowsableField = 0
            };

            var expectation = new ClassWithNonBrowsableMembers
            {
                BrowsableField = 1
            };

            // Act
            Action action =
                () => subject.Should().BeEquivalentTo(expectation, config => config.ExcludingNonBrowsableMembers());

            // Assert
            action.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_browsable_property_differs_excluding_non_browsable_members_should_not_affect_result()
        {
            // Arrange
            var subject = new ClassWithNonBrowsableMembers
            {
                BrowsableProperty = 0
            };

            var expectation = new ClassWithNonBrowsableMembers
            {
                BrowsableProperty = 1
            };

            // Act
            Action action =
                () => subject.Should().BeEquivalentTo(expectation, config => config.ExcludingNonBrowsableMembers());

            // Assert
            action.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_advanced_browsable_field_differs_excluding_non_browsable_members_should_not_affect_result()
        {
            // Arrange
            var subject = new ClassWithNonBrowsableMembers
            {
                AdvancedBrowsableField = 0
            };

            var expectation = new ClassWithNonBrowsableMembers
            {
                AdvancedBrowsableField = 1
            };

            // Act
            Action action =
                () => subject.Should().BeEquivalentTo(expectation, config => config.ExcludingNonBrowsableMembers());

            // Assert
            action.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_advanced_browsable_property_differs_excluding_non_browsable_members_should_not_affect_result()
        {
            // Arrange
            var subject = new ClassWithNonBrowsableMembers
            {
                AdvancedBrowsableProperty = 0
            };

            var expectation = new ClassWithNonBrowsableMembers
            {
                AdvancedBrowsableProperty = 1
            };

            // Act
            Action action =
                () => subject.Should().BeEquivalentTo(expectation, config => config.ExcludingNonBrowsableMembers());

            // Assert
            action.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_explicitly_browsable_field_differs_excluding_non_browsable_members_should_not_affect_result()
        {
            // Arrange
            var subject = new ClassWithNonBrowsableMembers
            {
                ExplicitlyBrowsableField = 0
            };

            var expectation = new ClassWithNonBrowsableMembers
            {
                ExplicitlyBrowsableField = 1
            };

            // Act
            Action action =
                () => subject.Should().BeEquivalentTo(expectation, config => config.ExcludingNonBrowsableMembers());

            // Assert
            action.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_explicitly_browsable_property_differs_excluding_non_browsable_members_should_not_affect_result()
        {
            // Arrange
            var subject = new ClassWithNonBrowsableMembers
            {
                ExplicitlyBrowsableProperty = 0
            };

            var expectation = new ClassWithNonBrowsableMembers
            {
                ExplicitlyBrowsableProperty = 1
            };

            // Act
            Action action =
                () => subject.Should().BeEquivalentTo(expectation, config => config.ExcludingNonBrowsableMembers());

            // Assert
            action.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_non_browsable_field_differs_excluding_non_browsable_members_should_make_it_succeed()
        {
            // Arrange
            var subject = new ClassWithNonBrowsableMembers
            {
                NonBrowsableField = 0
            };

            var expectation = new ClassWithNonBrowsableMembers
            {
                NonBrowsableField = 1
            };

            // Act & Assert
            subject.Should().BeEquivalentTo(expectation, config => config.ExcludingNonBrowsableMembers());
        }

        [Fact]
        public void When_non_browsable_property_differs_excluding_non_browsable_members_should_make_it_succeed()
        {
            // Arrange
            var subject = new ClassWithNonBrowsableMembers
            {
                NonBrowsableProperty = 0
            };

            var expectation = new ClassWithNonBrowsableMembers
            {
                NonBrowsableProperty = 1
            };

            // Act & Assert
            subject.Should().BeEquivalentTo(expectation, config => config.ExcludingNonBrowsableMembers());
        }

        [Fact]
        public void When_property_is_non_browsable_only_in_subject_excluding_non_browsable_members_should_not_make_it_succeed()
        {
            // Arrange
            var subject =
                new ClassWhereMemberThatCouldBeNonBrowsableIsNonBrowsable
                {
                    PropertyThatMightBeNonBrowsable = 0
                };

            var expectation =
                new ClassWhereMemberThatCouldBeNonBrowsableIsBrowsable
                {
                    PropertyThatMightBeNonBrowsable = 1
                };

            // Act
            Action action =
                () => subject.Should().BeEquivalentTo(expectation, config => config.ExcludingNonBrowsableMembers());

            // Assert
            action.Should().Throw<XunitException>()
                .WithMessage("Expected property subject.PropertyThatMightBeNonBrowsable to be 1, but found 0.*");
        }

        [Fact]
        public void
            When_property_is_non_browsable_only_in_subject_ignoring_non_browsable_members_on_subject_should_make_it_succeed()
        {
            // Arrange
            var subject =
                new ClassWhereMemberThatCouldBeNonBrowsableIsNonBrowsable
                {
                    PropertyThatMightBeNonBrowsable = 0
                };

            var expectation =
                new ClassWhereMemberThatCouldBeNonBrowsableIsBrowsable
                {
                    PropertyThatMightBeNonBrowsable = 1
                };

            // Act & Assert
            subject.Should().BeEquivalentTo(
                expectation,
                config => config.IgnoringNonBrowsableMembersOnSubject().ExcludingMissingMembers());
        }

        [Fact]
        public void When_non_browsable_property_on_subject_is_ignored_but_is_present_on_expectation_it_should_fail()
        {
            // Arrange
            var subject =
                new ClassWhereMemberThatCouldBeNonBrowsableIsNonBrowsable
                {
                    PropertyThatMightBeNonBrowsable = 0
                };

            var expectation =
                new ClassWhereMemberThatCouldBeNonBrowsableIsBrowsable
                {
                    PropertyThatMightBeNonBrowsable = 1
                };

            // Act
            Action action =
                () => subject.Should().BeEquivalentTo(expectation, config => config.IgnoringNonBrowsableMembersOnSubject());

            // Assert
            action.Should().Throw<XunitException>().WithMessage(
                "Expectation has*ThatMightBeNonBrowsable that is non-browsable in the other object, and non-browsable " +
                "members on the subject are ignored with the current configuration*");
        }

        [Fact]
        public void Only_ignore_non_browsable_matching_members()
        {
            // Arrange
            var subject = new
            {
                NonExisting = 0
            };

            var expectation = new
            {
                Existing = 1
            };

            // Act
            Action action = () => subject.Should().BeEquivalentTo(expectation, config => config.IgnoringNonBrowsableMembersOnSubject());

            // Assert
            action.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_property_is_non_browsable_only_in_expectation_excluding_non_browsable_members_should_make_it_succeed()
        {
            // Arrange
            var subject = new ClassWhereMemberThatCouldBeNonBrowsableIsBrowsable
            {
                PropertyThatMightBeNonBrowsable = 0
            };

            var expectation =
                new ClassWhereMemberThatCouldBeNonBrowsableIsNonBrowsable
                {
                    PropertyThatMightBeNonBrowsable = 1
                };

            // Act & Assert
            subject.Should().BeEquivalentTo(expectation, config => config.ExcludingNonBrowsableMembers());
        }

        [Fact]
        public void When_field_is_non_browsable_only_in_subject_excluding_non_browsable_members_should_not_make_it_succeed()
        {
            // Arrange
            var subject =
                new ClassWhereMemberThatCouldBeNonBrowsableIsNonBrowsable
                {
                    FieldThatMightBeNonBrowsable = 0
                };

            var expectation =
                new ClassWhereMemberThatCouldBeNonBrowsableIsBrowsable
                {
                    FieldThatMightBeNonBrowsable = 1
                };

            // Act
            Action action =
                () => subject.Should().BeEquivalentTo(expectation, config => config.ExcludingNonBrowsableMembers());

            // Assert
            action.Should().Throw<XunitException>()
                .WithMessage("Expected field subject.FieldThatMightBeNonBrowsable to be 1, but found 0.*");
        }

        [Fact]
        public void When_field_is_non_browsable_only_in_subject_ignoring_non_browsable_members_on_subject_should_make_it_succeed()
        {
            // Arrange
            var subject =
                new ClassWhereMemberThatCouldBeNonBrowsableIsNonBrowsable
                {
                    FieldThatMightBeNonBrowsable = 0
                };

            var expectation =
                new ClassWhereMemberThatCouldBeNonBrowsableIsBrowsable
                {
                    FieldThatMightBeNonBrowsable = 1
                };

            // Act & Assert
            subject.Should().BeEquivalentTo(
                expectation,
                config => config.IgnoringNonBrowsableMembersOnSubject().ExcludingMissingMembers());
        }

        [Fact]
        public void When_field_is_non_browsable_only_in_expectation_excluding_non_browsable_members_should_make_it_succeed()
        {
            // Arrange
            var subject = new ClassWhereMemberThatCouldBeNonBrowsableIsBrowsable
            {
                FieldThatMightBeNonBrowsable = 0
            };

            var expectation =
                new ClassWhereMemberThatCouldBeNonBrowsableIsNonBrowsable
                {
                    FieldThatMightBeNonBrowsable = 1
                };

            // Act & Assert
            subject.Should().BeEquivalentTo(expectation, config => config.ExcludingNonBrowsableMembers());
        }

        [Fact]
        public void When_property_is_missing_from_subject_excluding_non_browsable_members_should_make_it_succeed()
        {
            // Arrange
            var subject =
                new
                {
                    BrowsableField = 1,
                    BrowsableProperty = 1,
                    ExplicitlyBrowsableField = 1,
                    ExplicitlyBrowsableProperty = 1,
                    AdvancedBrowsableField = 1,
                    AdvancedBrowsableProperty = 1,
                    NonBrowsableField = 2
                    /* NonBrowsableProperty missing */
                };

            var expected = new ClassWithNonBrowsableMembers
            {
                BrowsableField = 1,
                BrowsableProperty = 1,
                ExplicitlyBrowsableField = 1,
                ExplicitlyBrowsableProperty = 1,
                AdvancedBrowsableField = 1,
                AdvancedBrowsableProperty = 1,
                NonBrowsableField = 2,
                NonBrowsableProperty = 2
            };

            // Act & Assert
            subject.Should().BeEquivalentTo(expected, opt => opt.ExcludingNonBrowsableMembers());
        }

        [Fact]
        public void When_field_is_missing_from_subject_excluding_non_browsable_members_should_make_it_succeed()
        {
            // Arrange
            var subject =
                new
                {
                    BrowsableField = 1,
                    BrowsableProperty = 1,
                    ExplicitlyBrowsableField = 1,
                    ExplicitlyBrowsableProperty = 1,
                    AdvancedBrowsableField = 1,
                    AdvancedBrowsableProperty = 1,
                    /* NonBrowsableField missing */
                    NonBrowsableProperty = 2
                };

            var expected = new ClassWithNonBrowsableMembers
            {
                BrowsableField = 1,
                BrowsableProperty = 1,
                ExplicitlyBrowsableField = 1,
                ExplicitlyBrowsableProperty = 1,
                AdvancedBrowsableField = 1,
                AdvancedBrowsableProperty = 1,
                NonBrowsableField = 2,
                NonBrowsableProperty = 2
            };

            // Act & Assert
            subject.Should().BeEquivalentTo(expected, opt => opt.ExcludingNonBrowsableMembers());
        }

        [Fact]
        public void When_property_is_missing_from_expectation_excluding_non_browsable_members_should_make_it_succeed()
        {
            // Arrange
            var subject = new ClassWithNonBrowsableMembers
            {
                BrowsableField = 1,
                BrowsableProperty = 1,
                ExplicitlyBrowsableField = 1,
                ExplicitlyBrowsableProperty = 1,
                AdvancedBrowsableField = 1,
                AdvancedBrowsableProperty = 1,
                NonBrowsableField = 2,
                NonBrowsableProperty = 2
            };

            var expected =
                new
                {
                    BrowsableField = 1,
                    BrowsableProperty = 1,
                    ExplicitlyBrowsableField = 1,
                    ExplicitlyBrowsableProperty = 1,
                    AdvancedBrowsableField = 1,
                    AdvancedBrowsableProperty = 1,
                    NonBrowsableField = 2
                    /* NonBrowsableProperty missing */
                };

            // Act & Assert
            subject.Should().BeEquivalentTo(expected, opt => opt.ExcludingNonBrowsableMembers());
        }

        [Fact]
        public void When_field_is_missing_from_expectation_excluding_non_browsable_members_should_make_it_succeed()
        {
            // Arrange
            var subject = new ClassWithNonBrowsableMembers
            {
                BrowsableField = 1,
                BrowsableProperty = 1,
                ExplicitlyBrowsableField = 1,
                ExplicitlyBrowsableProperty = 1,
                AdvancedBrowsableField = 1,
                AdvancedBrowsableProperty = 1,
                NonBrowsableField = 2,
                NonBrowsableProperty = 2
            };

            var expected =
                new
                {
                    BrowsableField = 1,
                    BrowsableProperty = 1,
                    ExplicitlyBrowsableField = 1,
                    ExplicitlyBrowsableProperty = 1,
                    AdvancedBrowsableField = 1,
                    AdvancedBrowsableProperty = 1,
                    /* NonBrowsableField missing */
                    NonBrowsableProperty = 2
                };

            // Act & Assert
            subject.Should().BeEquivalentTo(expected, opt => opt.ExcludingNonBrowsableMembers());
        }

        [Fact]
        public void
            When_non_browsable_members_are_excluded_it_should_still_be_possible_to_explicitly_include_non_browsable_field()
        {
            // Arrange
            var subject = new ClassWithNonBrowsableMembers
            {
                NonBrowsableField = 1
            };

            var expectation = new ClassWithNonBrowsableMembers
            {
                NonBrowsableField = 2
            };

            // Act
            Action action =
                () => subject.Should().BeEquivalentTo(
                    expectation,
                    opt => opt.IncludingFields().ExcludingNonBrowsableMembers().Including(e => e.NonBrowsableField));

            // Assert
            action.Should().Throw<XunitException>()
                .WithMessage("Expected field subject.NonBrowsableField to be 2, but found 1.*");
        }

        [Fact]
        public void
            When_non_browsable_members_are_excluded_it_should_still_be_possible_to_explicitly_include_non_browsable_property()
        {
            // Arrange
            var subject = new ClassWithNonBrowsableMembers
            {
                NonBrowsableProperty = 1
            };

            var expectation = new ClassWithNonBrowsableMembers
            {
                NonBrowsableProperty = 2
            };

            // Act
            Action action =
                () => subject.Should().BeEquivalentTo(
                    expectation,
                    opt => opt.IncludingProperties().ExcludingNonBrowsableMembers().Including(e => e.NonBrowsableProperty));

            // Assert
            action.Should().Throw<XunitException>()
                .WithMessage("Expected property subject.NonBrowsableProperty to be 2, but found 1.*");
        }

        private class ClassWithNonBrowsableMembers
        {
            [UsedImplicitly]
            public int BrowsableField = -1;

            [UsedImplicitly]
            public int BrowsableProperty { get; set; }

            [UsedImplicitly]
            [EditorBrowsable(EditorBrowsableState.Always)]
            public int ExplicitlyBrowsableField = -1;

            [UsedImplicitly]
            [EditorBrowsable(EditorBrowsableState.Always)]
            public int ExplicitlyBrowsableProperty { get; set; }

            [UsedImplicitly]
            [EditorBrowsable(EditorBrowsableState.Advanced)]
            public int AdvancedBrowsableField = -1;

            [UsedImplicitly]
            [EditorBrowsable(EditorBrowsableState.Advanced)]
            public int AdvancedBrowsableProperty { get; set; }

            [UsedImplicitly]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public int NonBrowsableField = -1;

            [UsedImplicitly]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public int NonBrowsableProperty { get; set; }
        }

        private class ClassWhereMemberThatCouldBeNonBrowsableIsBrowsable
        {
            [UsedImplicitly]
            public int BrowsableProperty { get; set; }

            [UsedImplicitly]
            public int FieldThatMightBeNonBrowsable = -1;

            [UsedImplicitly]
            public int PropertyThatMightBeNonBrowsable { get; set; }
        }

        private class ClassWhereMemberThatCouldBeNonBrowsableIsNonBrowsable
        {
            [UsedImplicitly]
            public int BrowsableProperty { get; set; }

            [EditorBrowsable(EditorBrowsableState.Never)]
            [UsedImplicitly]
            public int FieldThatMightBeNonBrowsable = -1;

            [EditorBrowsable(EditorBrowsableState.Never)]
            [UsedImplicitly]
            public int PropertyThatMightBeNonBrowsable { get; set; }
        }
    }
}
