using System;
using System.Text;

namespace AwesomeAssertions.Equivalency.Tracing;

/// <summary>
/// An <see cref="ITraceWriter"/> that collects the trace of a structural equivalency comparison in a
/// <see cref="StringBuilder"/>.
/// </summary>
public class StringBuilderTraceWriter : ITraceWriter
{
    private readonly StringBuilder builder = new();
    private int depth = 1;

    /// <inheritdoc />
    public void AddSingle(string trace)
    {
        WriteLine(trace);
    }

    /// <inheritdoc />
    public IDisposable AddBlock(string trace)
    {
        WriteLine(trace);
        WriteLine("{");
        depth++;

        return new Disposable(() =>
        {
            depth--;
            WriteLine("}");
        });
    }

    private void WriteLine(string trace)
    {
        foreach (string traceLine in trace.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries))
        {
            builder.Append(' ', depth * 2).AppendLine(traceLine);
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return builder.ToString();
    }
}
