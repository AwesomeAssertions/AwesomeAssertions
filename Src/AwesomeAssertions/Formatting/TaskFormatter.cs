using System.Threading.Tasks;

namespace AwesomeAssertions.Formatting;

/// <summary>
/// Provides a human-readable version of a generic or non-generic <see cref="Task"/>
/// including its state.
/// </summary>
public class TaskFormatter : IValueFormatter
{
    /// <inheritdoc />
    public bool CanHandle(object value)
    {
        return value is Task;
    }

    /// <inheritdoc />
    public void Format(object value, FormattedObjectGraph formattedGraph, FormattingContext context, FormatChild formatChild)
    {
        var task = (Task)value;
        formatChild("type", task.GetType(), formattedGraph);
        formattedGraph.AddFragment($" {{Status={task.Status}}}");
    }
}
