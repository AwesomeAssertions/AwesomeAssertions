using System;
using Xunit;

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

            subject.Should().BeEquivalentTo(expected, o => o.ExcludeObsoleteMembers());
        }

        private class ClassWithObsoleteMembers
        {
            public string StringProperty { get; set; }

            [Obsolete("This property is obsolete and will be removed in a future version.")]
            public string ObsoleteProperty { get; set; }
        }
    }
}
