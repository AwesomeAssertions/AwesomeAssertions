using System;
using System.Linq;
using System.Text;
using System.Threading;
using FluentAssertions.Common;
using FluentAssertions.Formatting;

namespace FluentAssertions.Execution;

/// <summary>
/// Represents an implicit or explicit scope within which multiple assertions can be collected.
/// </summary>
/// <remarks>
/// This class is supposed to have a very short lifetime and is not safe to be used in assertion that cross thread-boundaries
/// such as when using <see langword="async"/> or <see langword="await"/>.
/// </remarks>
public sealed class AssertionScope : IDisposable
{
    private readonly IAssertionStrategy assertionStrategy;

    /// <summary>
    /// The default scopes, which were implicitly created by accessing <see cref="Current"/>.
    /// </summary>
    private static readonly AsyncLocal<AssertionScope> DefaultScope = new();
    private static readonly AsyncLocal<AssertionScope> CurrentScope = new();
    private readonly Func<string> callerIdentityProvider = () => CallerIdentifier.DetermineCallerIdentity();
    private readonly ContextDataDictionary reportableData = new();
    private readonly StringBuilder tracing = new();

#pragma warning disable CA2213 // Disposable fields should be disposed
    private AssertionScope parent;
#pragma warning restore CA2213

    /// <summary>
    /// Starts an unnamed scope within which multiple assertions can be executed
    /// and which will not throw until the scope is disposed.
    /// </summary>
    public AssertionScope()
        : this(() => null, new CollectingAssertionStrategy())
    {
    }

    /// <summary>
    /// Starts a named scope within which multiple assertions can be executed
    /// and which will not throw until the scope is disposed.
    /// </summary>
    public AssertionScope(string name)
        : this(() => name, new CollectingAssertionStrategy())
    {
    }

    /// <summary>
    /// Starts a new scope based on the given assertion strategy.
    /// </summary>
    /// <param name="assertionStrategy">The assertion strategy for this scope.</param>
    /// <exception cref="ArgumentNullException"><paramref name="assertionStrategy"/> is <see langword="null"/>.</exception>
    public AssertionScope(IAssertionStrategy assertionStrategy)
        : this(() => null, assertionStrategy)
    {
    }

    /// <summary>
    /// Starts a named scope within which multiple assertions can be executed
    /// and which will not throw until the scope is disposed.
    /// </summary>
    public AssertionScope(Func<string> name)
        : this(name, new CollectingAssertionStrategy())
    {
    }

    /// <summary>
    /// Starts a new scope based on the given assertion strategy and parent assertion scope
    /// </summary>
    /// <param name="assertionStrategy">The assertion strategy for this scope.</param>
    /// <exception cref="ArgumentNullException"><paramref name="assertionStrategy"/> is <see langword="null"/>.</exception>
    private AssertionScope(Func<string> name, IAssertionStrategy assertionStrategy)
    {
        parent = GetParentScope();
        CurrentScope.Value = this;

        this.assertionStrategy = assertionStrategy
            ?? throw new ArgumentNullException(nameof(assertionStrategy));

        if (parent is not null)
        {
            // Combine the existing Name with the parent.Name if it exists.
            Name = () =>
            {
                var parentName = parent.Name();
                if (parentName.IsNullOrEmpty())
                {
                    return name();
                }

                if (name().IsNullOrEmpty())
                {
                    return parentName;
                }

                return parentName + "/" + name();
            };

            callerIdentityProvider = parent.callerIdentityProvider;
            FormattingOptions = parent.FormattingOptions.Clone();
        }
        else
        {
            Name = name;
        }
    }

    /// <summary>
    /// Get parent scope.
    /// <para>
    /// The parent scope is any explicitly opened <see cref="AssertionScope"/>, except
    /// for the implicitly opened through access to <see cref="AssertionScope.Current"/> or
    /// <see cref="AssertionChain.GetOrCreate" />.
    /// </para>
    /// </summary>
    /// <returns>The parent scope</returns>
    private static AssertionScope GetParentScope()
    {
        if (CurrentScope.Value is not null && CurrentScope.Value != DefaultScope.Value)
        {
            return CurrentScope.Value;
        }

        return null;
    }

    /// <summary>
    /// Gets or sets the name of the current assertion scope, e.g. the path of the object graph
    /// that is being asserted on.
    /// </summary>
    /// <remarks>
    /// The context is provided by a <see cref="Lazy{String}"/> which
    /// only gets evaluated when its value is actually needed (in most cases during a failure).
    /// </remarks>
    public Func<string> Name { get; }

    /// <summary>
    /// Gets the current thread-specific assertion scope.
    /// </summary>
    public static AssertionScope Current
    {
        get
        {
            if (CurrentScope.Value is not null)
            {
                return CurrentScope.Value;
            }

            DefaultScope.Value ??= new AssertionScope(() => null, new DefaultAssertionStrategy());
            return DefaultScope.Value;
        }
    }

    /// <summary>
    /// Exposes the options the scope will use for formatting objects in case an assertion fails.
    /// </summary>
    public FormattingOptions FormattingOptions { get; } = AssertionConfiguration.Current.Formatting.Clone();

    /// <summary>
    /// Adds a pre-formatted failure message to the current scope.
    /// </summary>
    public void AddPreFormattedFailure(string formattedFailureMessage)
    {
        assertionStrategy.HandleFailure(formattedFailureMessage);
    }

    /// <summary>
    /// Adds some information to the assertion scope that will be included in the message
    /// that is emitted if an assertion fails.
    /// </summary>
    internal void AddReportable(string key, string value)
    {
        reportableData.Add(new ContextDataDictionary.DataItem(key, value, reportable: true, requiresFormatting: false));
    }

    /// <summary>
    /// Adds some information to the assertion scope that will be included in the message
    /// that is emitted if an assertion fails. The value is only calculated on failure.
    /// </summary>
    internal void AddReportable(string key, Func<string> valueFunc)
    {
        reportableData.Add(new ContextDataDictionary.DataItem(key, new DeferredReportable(valueFunc), reportable: true,
            requiresFormatting: false));
    }

    /// <summary>
    /// Adds a block of tracing to the scope for reporting when an assertion fails.
    /// </summary>
    public void AppendTracing(string tracingBlock)
    {
        tracing.Append(tracingBlock);
    }

    /// <summary>
    /// Returns all failures that happened up to this point and ensures they will not cause
    /// <see cref="Dispose"/> to fail the assertion.
    /// </summary>
    public string[] Discard()
    {
        return assertionStrategy.DiscardFailures().ToArray();
    }

    public bool HasFailures()
    {
        return assertionStrategy.FailureMessages.Any();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        CurrentScope.Value = parent;

        if (parent is not null)
        {
            foreach (string failureMessage in assertionStrategy.FailureMessages)
            {
                parent.assertionStrategy.HandleFailure(failureMessage);
            }

            parent.reportableData.Add(reportableData);
            parent.AppendTracing(tracing.ToString());

            parent = null;
        }
        else
        {
            if (tracing.Length > 0)
            {
                reportableData.Add(new ContextDataDictionary.DataItem("trace", tracing.ToString(), reportable: true, requiresFormatting: false));
            }

            assertionStrategy.ThrowIfAny(reportableData.GetReportable());
        }
    }

    private sealed class DeferredReportable(Func<string> valueFunc)
    {
        private readonly Lazy<string> lazyValue = new(valueFunc);

        public override string ToString() => lazyValue.Value;
    }
}
