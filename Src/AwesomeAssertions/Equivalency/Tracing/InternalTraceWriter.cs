namespace AwesomeAssertions.Equivalency.Tracing;

/// <summary>
/// A trace writer that is used internally as default by the equivalency assertion engine.
/// </summary>
/// <remarks>
/// This is used to decide if the trace writer was set by the user or as default by us.
/// </remarks>
internal sealed class InternalTraceWriter : StringBuilderTraceWriter;
