using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AwesomeAssertions.Common;
using AwesomeAssertions.Equivalency.Selection;

namespace AwesomeAssertions.Equivalency;

/// <summary>
/// Provides a fluent API for excluding a nested member reached by navigating through one or more collections
/// on the <typeparamref name="TExpectation"/> object graph.
/// </summary>
/// <typeparam name="TExpectation">The type of the root expectation the exclusions are configured on.</typeparam>
/// <typeparam name="TCurrent">The item type of the collection currently selected in the navigation chain.</typeparam>
public class NestedExclusionOptionBuilder<TExpectation, TCurrent>
{
    /// <summary>
    /// The selected path starting at the first <see cref="EquivalencyOptions{TExpectation}.For{TNext}"/>.
    /// </summary>
    private readonly ExcludeMemberByPathSelectionRule currentPathSelectionRule;

    private readonly EquivalencyOptions<TExpectation> capturedOptions;

    internal NestedExclusionOptionBuilder(EquivalencyOptions<TExpectation> capturedOptions,
        ExcludeMemberByPathSelectionRule currentPathSelectionRule)
    {
        this.capturedOptions = capturedOptions;
        this.currentPathSelectionRule = currentPathSelectionRule;
    }

    /// <summary>
    /// Selects a nested property to exclude. This ends the <see cref="For{TNext}"/> chain.
    /// </summary>
    public EquivalencyOptions<TExpectation> Exclude(Expression<Func<TCurrent, object>> expression)
    {
        var currentSelectionPath = currentPathSelectionRule.CurrentPath;

        foreach (var path in expression.GetMemberPaths())
        {
            var newPath = currentSelectionPath.AsParentCollectionOf(path);
            capturedOptions.AddSelectionRule(new ExcludeMemberByPathSelectionRule(newPath));
        }

        return capturedOptions;
    }

    /// <summary>
    /// Adds the selected collection to the <see cref="For{TNext}"/> chain.
    /// </summary>
    public NestedExclusionOptionBuilder<TExpectation, TNext> For<TNext>(
        Expression<Func<TCurrent, IEnumerable<TNext>>> expression)
    {
        var nextPath = expression.GetMemberPath();
        currentPathSelectionRule.AppendPath(nextPath);
        return new NestedExclusionOptionBuilder<TExpectation, TNext>(capturedOptions, currentPathSelectionRule);
    }
}
