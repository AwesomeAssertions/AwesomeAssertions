using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Streams;

/// <summary>
/// Contains a number of methods to assert that an <see cref="Stream"/> is in the expected state.
/// </summary>
///
[DebuggerNonUserCode]
public class BufferedStreamAssertions : BufferedStreamAssertions<BufferedStreamAssertions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BufferedStreamAssertions"/> class.
    /// </summary>
    /// <param name="stream">The <see cref="BufferedStream"/> to assert on.</param>
    /// <param name="assertionChain">
    /// The <see cref="AssertionChain"/> that manages the state of the assertion, including the reason and identifier.
    /// </param>
    public BufferedStreamAssertions(BufferedStream stream, AssertionChain assertionChain)
        : base(stream, assertionChain)
    {
    }
}

/// <summary>
/// Contains a number of methods to assert that a <see cref="BufferedStream"/> is in the expected state.
/// </summary>
public class BufferedStreamAssertions<TAssertions> : StreamAssertions<BufferedStream, TAssertions>
    where TAssertions : BufferedStreamAssertions<TAssertions>
{
#if NET6_0_OR_GREATER || NETSTANDARD2_1

    private readonly AssertionChain assertionChain;

    /// <summary>
    /// Initializes a new instance of the <see cref="BufferedStreamAssertions{TAssertions}"/> class.
    /// </summary>
    /// <param name="stream">The <see cref="BufferedStream"/> to assert on.</param>
    /// <param name="assertionChain">
    /// The <see cref="AssertionChain"/> that manages the state of the assertion, including the reason and identifier.
    /// </param>
    public BufferedStreamAssertions(BufferedStream stream, AssertionChain assertionChain)
        : base(stream, assertionChain)
    {
        this.assertionChain = assertionChain;
    }

    /// <summary>
    /// Asserts that the current <see cref="BufferedStream"/> has the <paramref name="expected"/> buffer size.
    /// </summary>
    /// <param name="expected">The expected buffer size of the current stream.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    [return: NotNull]
    public AndConstraint<TAssertions> HaveBufferSize(int expected,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        assertionChain
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject is not null)
            .FailWith("Expected the buffer size of {context:stream} to be {0}{reason}, but found a <null> reference.",
                expected);

        if (assertionChain.Succeeded)
        {
            assertionChain
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject!.BufferSize == expected)
                .FailWith("Expected the buffer size of {context:stream} to be {0}{reason}, but it was {1}.",
                    expected, Subject.BufferSize);
        }

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    /// <summary>
    /// Asserts that the current <see cref="BufferedStream"/> does not have a buffer size of <paramref name="unexpected"/>.
    /// </summary>
    /// <param name="unexpected">The unexpected buffer size of the current stream.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    [return: NotNull]
    public AndConstraint<TAssertions> NotHaveBufferSize(int unexpected,
        [StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        assertionChain
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject is not null)
            .FailWith("Expected the buffer size of {context:stream} not to be {0}{reason}, but found a <null> reference.",
                unexpected);

        if (assertionChain.Succeeded)
        {
            assertionChain
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject!.BufferSize != unexpected)
                .FailWith("Expected the buffer size of {context:stream} not to be {0}{reason}, but it was.",
                    unexpected);
        }

        return new AndConstraint<TAssertions>((TAssertions)this);
    }
#else
    /// <summary>
    /// Initializes a new instance of the <see cref="BufferedStreamAssertions{TAssertions}"/> class.
    /// </summary>
    /// <param name="stream">The <see cref="BufferedStream"/> to assert on.</param>
    /// <param name="assertionChain">
    /// The <see cref="AssertionChain"/> that manages the state of the assertion, including the reason and identifier.
    /// </param>
    public BufferedStreamAssertions(BufferedStream stream, AssertionChain assertionChain)
        : base(stream, assertionChain)
    {
    }
#endif

    /// <inheritdoc />
    protected override string Identifier => "buffered stream";
}
