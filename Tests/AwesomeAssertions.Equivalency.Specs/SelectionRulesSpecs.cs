using System;
using AwesomeAssertions.Equivalency.Matching;
using AwesomeAssertions.Equivalency.Ordering;
using AwesomeAssertions.Equivalency.Selection;
using Xunit;

namespace AwesomeAssertions.Equivalency.Specs;

public partial class SelectionRulesSpecs
{
    [Fact]
    public void Public_methods_follow_fluent_syntax()
    {
        // Arrange
        var subject = new Root();
        var expected = new RootDto();

        // Act / Assert
        subject.Should().BeEquivalentTo(expected,
            options => options
                .AllowingInfiniteRecursion()
                .ComparingByMembers(typeof(Root))
                .ComparingByMembers<RootDto>()
                .ComparingByValue(typeof(Customer))
                .ComparingByValue<CustomerDto>()
                .ComparingEnumsByName()
                .ComparingEnumsByValue()
                .ComparingRecordsByMembers()
                .ComparingRecordsByValue()
                .Excluding(r => r.Level)
                .ExcludingFields()
                .ExcludingMissingMembers()
                .WithoutRecursing()
                .ExcludingNonBrowsableMembers()
                .ExcludingProperties()
                .IgnoringCyclicReferences()
                .IgnoringNonBrowsableMembersOnSubject()
                .Including(r => r.Level)
                .IncludingAllDeclaredProperties()
                .IncludingAllRuntimeProperties()
                .IncludingFields()
                .IncludingInternalFields()
                .IncludingInternalProperties()
                .IncludingNestedObjects()
                .IncludingProperties()
                .PreferringDeclaredMemberTypes()
                .PreferringRuntimeMemberTypes()
                .ThrowingOnMissingMembers()
                .Using(new ExtensibilitySpecs.DoEquivalencyStep(() => { }))
                .Using(new MustMatchByNameRule())
                .Using(new AllFieldsSelectionRule())
                .Using(new ByteArrayOrderingRule())
                .Using(StringComparer.OrdinalIgnoreCase)
                .WithAutoConversion()
                .WithAutoConversionFor(_ => false)
                .WithoutAutoConversionFor(_ => true)
                .WithoutMatchingRules()
                .WithoutSelectionRules()
                .WithoutStrictOrdering()
                .WithoutStrictOrderingFor(r => r.Level)
                .WithStrictOrdering()
                .WithStrictOrderingFor(r => r.Level)
                .WithTracing()
            );
    }
}
