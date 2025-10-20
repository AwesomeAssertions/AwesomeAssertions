using System;
using System.Collections.Generic;
using System.Linq;

namespace AwesomeAssertions.Equivalency.Selection;

/// <summary>
/// Selection rule that removes a particular member from the structural comparison if its name matches the specified criteria.
/// </summary>
internal class ExcludeMemberByNameSelectionRule : IMemberSelectionRule
{
    private readonly Func<string, bool> predicate;
    private readonly string[] memberNames;

    public ExcludeMemberByNameSelectionRule(string[] memberNames)
    {
        this.memberNames = memberNames;
        predicate = name => memberNames.Contains(name, StringComparer.Ordinal);
    }

    public bool IncludesMembers => false;

    public IEnumerable<IMember> SelectMembers(INode currentNode, IEnumerable<IMember> selectedMembers,
        MemberSelectionContext context)
    {
        return selectedMembers.Where(p => !predicate(p.Expectation.Name)).ToArray();
    }

    /// <inheritdoc />
    /// <filterpriority>2</filterpriority>
    public override string ToString()
    {
        return $"Exclude members named: {string.Join(", ", memberNames)}";
    }
}
