using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;
using Xunit.Sdk;
using static AwesomeAssertions.FluentActions;

namespace AwesomeAssertions.Equivalency.Specs;

#pragma warning disable CS0618 // Ignore obsolete warning because we explicitly want to test this
public partial class SelectionRulesSpecs
{
    public class Obsolete
    {
        [Fact]
        public void When_obsolete_property_differs_comparison_should_ignore_it()
        {
            var subject = new ClassWithObsoleteMembers { ObsoleteProperty = "SubjectValue" };
            var expected = new ClassWithObsoleteMembers { ObsoleteProperty = "ExpectedValue" };

            subject.Should().BeEquivalentTo(expected, o => o.ExcludingObsoleteMembers());
        }

        [Fact]
        public void When_obsolete_field_differs_comparison_should_ignore_it()
        {
            var subject = new ClassWithObsoleteMembers { ObsoleteField = "SubjectValue" };
            var expected = new ClassWithObsoleteMembers { ObsoleteField = "ExpectedValue" };

            subject.Should().BeEquivalentTo(expected, o => o.ExcludingObsoleteMembers());
        }

        [Fact]
        public void When_obsolete_property_is_missing_comparison_should_ignore_it()
        {
            var subject = new { StringProperty = "String", ObsoleteField = (string)null };
            var expected = new ClassWithObsoleteMembers { StringProperty = "String", ObsoleteProperty = "ExpectedValue" };

            subject.Should().BeEquivalentTo(expected, o => o.ExcludingObsoleteMembers());
        }

        [Fact]
        public void When_obsolete_property_is_defined_on_the_subject_it_is_not_excluded()
        {
            var subject = new ClassWithObsoleteMembers { ObsoleteProperty = "SubjectValue" };
            var expected = new { ObsoleteProperty = "ExpectedValue" };

            Action act = () => subject.Should().BeEquivalentTo(
                expected, o => o.ExcludingObsoleteMembers(),
                "we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>().WithMessage("*failure message*SubjectValue*ExpectedValue*");
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        [SuppressMessage("ReSharper", "NotAccessedField.Local")]
        private class ClassWithObsoleteMembers
        {
            public string StringProperty { get; set; }

            [Obsolete("This property is obsolete and will be removed in a future version.")]
            public string ObsoleteProperty { get; set; }

            [Obsolete("This property is obsolete and will be removed in a future version.")]
            public string ObsoleteField;
        }
    }
}
