using System.Reflection;
using AwesomeAssertions.Common;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Equivalency.Matching;

/// <summary>
/// Finds a member of the expectation with the exact same name, but doesn't require it.
/// </summary>
internal class TryMatchByNameRule : IMemberMatchingRule
{
    public IMember Match(IMember expectedMember, object subject, INode parent, IEquivalencyOptions options, AssertionChain assertionChain)
    {
        if (options.IncludedProperties != MemberVisibility.None)
        {
            PropertyInfo property = subject.GetType().FindProperty(expectedMember.Expectation.Name,
                options.IncludedProperties | MemberVisibility.ExplicitlyImplemented);

            if (property is not null && !property.IsIndexer())
            {
                return new Property(property, parent);
            }
        }

        FieldInfo field = subject.GetType()
            .FindField(expectedMember.Expectation.Name, options.IncludedFields);

        return field is not null ? new Field(field, parent) : null;
    }

    /// <inheritdoc />
    /// <filterpriority>2</filterpriority>
    public override string ToString()
    {
        return "Try to match member by name";
    }
}
