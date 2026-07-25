using AwesomeAssertions.Equivalency.Execution;
using AwesomeAssertions.Equivalency.Tracing;
using AwesomeAssertions.Execution;
using static System.FormattableString;

namespace AwesomeAssertions.Equivalency;

/// <summary>
/// Provides information on a particular property during an assertion for structural equality of two object graphs.
/// </summary>
public class EquivalencyValidationContext : IEquivalencyValidationContext
{
    private Tracer tracer;

    /// <summary>
    /// Initializes a new instance of the <see cref="EquivalencyValidationContext"/> class.
    /// </summary>
    /// <param name="root">The node representing the root object of the object graph that is being validated.</param>
    /// <param name="options">The options that control how the structural equivalency is asserted.</param>
    public EquivalencyValidationContext(INode root, IEquivalencyOptions options)
    {
        Options = options;
        CurrentNode = root;
        CyclicReferenceDetector = new CyclicReferenceDetector();
    }

    /// <inheritdoc />
    public INode CurrentNode { get; }

    /// <inheritdoc />
    public Reason Reason { get; set; }

    /// <inheritdoc />
    public Tracer Tracer => tracer ??= new Tracer(CurrentNode, TraceWriter);

    /// <inheritdoc />
    public IEquivalencyOptions Options { get; }

    private CyclicReferenceDetector CyclicReferenceDetector { get; set; }

    /// <inheritdoc />
    public IEquivalencyValidationContext AsNestedMember(IMember expectationMember)
    {
        return new EquivalencyValidationContext(expectationMember, Options)
        {
            Reason = Reason,
            TraceWriter = TraceWriter,
            CyclicReferenceDetector = CyclicReferenceDetector
        };
    }

    /// <inheritdoc />
    public IEquivalencyValidationContext AsCollectionItem<TItem>(string index)
    {
        return new EquivalencyValidationContext(Node.FromCollectionItem<TItem>(index, CurrentNode), Options)
        {
            Reason = Reason,
            TraceWriter = TraceWriter,
            CyclicReferenceDetector = CyclicReferenceDetector
        };
    }

    /// <inheritdoc />
    public IEquivalencyValidationContext AsDictionaryItem<TKey, TExpectation>(TKey key)
    {
        return new EquivalencyValidationContext(Node.FromDictionaryItem<TExpectation>(key, CurrentNode), Options)
        {
            Reason = Reason,
            TraceWriter = TraceWriter,
            CyclicReferenceDetector = CyclicReferenceDetector
        };
    }

    /// <inheritdoc />
    public IEquivalencyValidationContext Clone()
    {
        return new EquivalencyValidationContext(CurrentNode, Options)
        {
            Reason = Reason,
            TraceWriter = TraceWriter,
            CyclicReferenceDetector = CyclicReferenceDetector
        };
    }

    /// <inheritdoc />
    public bool IsCyclicReference(object expectation)
    {
        bool compareByMembers = expectation is not null && Options.GetEqualityStrategy(expectation.GetType())
            is EqualityStrategy.Members or EqualityStrategy.ForceMembers;

        var reference = new ObjectReference(expectation, CurrentNode.Subject.PathAndName, compareByMembers);
        return CyclicReferenceDetector.IsCyclicReference(reference);
    }

    /// <summary>
    /// Gets or sets the <see cref="ITraceWriter"/> used to collect tracing messages produced during the equivalency validation.
    /// </summary>
    public ITraceWriter TraceWriter { get; set; }

    /// <summary>
    /// This method ensures that tracing starts with a fresh state when invoked.
    /// </summary>
    internal void ResetTracing()
    {
        // SMELL: We need to ensure that if tracing is enabled using the built-in internal writer,
        // we start with a fresh instance of InternalTraceWriter. We can't add extend ITraceWriter
        // as that would be a breaking change.
        if (TraceWriter is InternalTraceWriter)
        {
            TraceWriter = new InternalTraceWriter();
        }
    }

    /// <summary>
    /// Returns a string representation of the path to the <see cref="CurrentNode"/> that is currently being validated.
    /// </summary>
    public override string ToString()
    {
        return Invariant($"{{Path=\"{CurrentNode.Subject.PathAndName}\"}}");
    }
}
