using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace AwesomeAssertions.Execution;

internal class CollectingAssertionStrategy : IAssertionStrategy
{
    private readonly List<string> failureMessages = [];

    /// <summary>
    /// Returns the messages for the assertion failures that happened until now.
    /// </summary>
    public IEnumerable<string> FailureMessages => failureMessages;

    /// <summary>
    /// Discards and returns the failure messages that happened up to now.
    /// </summary>
    public IEnumerable<string> DiscardFailures()
    {
        var discardedFailures = failureMessages.ToArray();
        failureMessages.Clear();
        return discardedFailures;
    }

    /// <summary>
    /// Will throw a combined exception for any failures have been collected.
    /// </summary>
    [StackTraceHidden]
    public void ThrowIfAny(IDictionary<string, object> context)
    {
        if (failureMessages.Count > 0)
        {
            var builder = new StringBuilder();
            builder.AppendJoin(Environment.NewLine, failureMessages).AppendLine();

            if (context.Any())
            {
                foreach (KeyValuePair<string, object> pair in context)
                {
                    builder.AppendFormat(CultureInfo.InvariantCulture,
                        $"{Environment.NewLine}With {pair.Key}:{Environment.NewLine}{{0}}", pair.Value);
                }
            }

            AssertionEngine.TestFramework.Throw(builder.ToString());
        }
    }

    /// <summary>
    /// Instructs the strategy to handle a assertion failure.
    /// </summary>
    public void HandleFailure(string message)
    {
        failureMessages.Add(message);
    }
}
