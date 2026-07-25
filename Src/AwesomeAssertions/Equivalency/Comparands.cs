using System;
using AwesomeAssertions.Common;
using static System.FormattableString;

namespace AwesomeAssertions.Equivalency;

/// <summary>
/// Holds the subject and expectation that are currently being compared, together with the compile-time type
/// through which the expectation was declared.
/// </summary>
public class Comparands
{
    private Type compileTimeType;

    /// <summary>
    /// Initializes a new instance of the <see cref="Comparands"/> class.
    /// </summary>
    public Comparands()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Comparands"/> class.
    /// </summary>
    /// <param name="subject">The value of the subject object graph.</param>
    /// <param name="expectation">The value of the expected object graph.</param>
    /// <param name="compileTimeType">The declared (compile-time) type of the <paramref name="expectation"/>.</param>
    public Comparands(object subject, object expectation, Type compileTimeType)
    {
        this.compileTimeType = compileTimeType;
        Subject = subject;
        Expectation = expectation;
    }

    /// <summary>
    /// Gets the value of the subject object graph.
    /// </summary>
    public object Subject { get; set; }

    /// <summary>
    /// Gets the value of the expected object graph.
    /// </summary>
    public object Expectation { get; set; }

    /// <summary>
    /// Gets or sets the compile-time type of the <see cref="Expectation"/>, falling back to the <see cref="RuntimeType"/>
    /// when the declared type is <see cref="object"/> and an expectation value is available.
    /// </summary>
    public Type CompileTimeType
    {
        get
        {
            return compileTimeType != typeof(object) || Expectation is null ? compileTimeType : RuntimeType;
        }

        // SMELL: Do we really need this? Can we replace it by making Comparands generic or take a constructor parameter?
        set => compileTimeType = value;
    }

    /// <summary>
    /// Gets the run-time type of the current expectation object.
    /// </summary>
    public Type RuntimeType
    {
        get
        {
            if (Expectation is not null)
            {
                return Expectation.GetType();
            }

            return CompileTimeType;
        }
    }

    /// <summary>
    /// Returns either the run-time or compile-time type of the expectation based on the options provided by the caller.
    /// </summary>
    /// <remarks>
    /// If the expectation is a nullable type, it should return the type of the wrapped object.
    /// </remarks>
    public Type GetExpectedType(IEquivalencyOptions options)
    {
        Type type = options.UseRuntimeTyping ? RuntimeType : CompileTimeType;

        return type.NullableOrActualType();
    }

    /// <summary>
    /// Returns a string representation of the <see cref="Subject"/> and <see cref="Expectation"/> being compared.
    /// </summary>
    public override string ToString()
    {
        return Invariant($"{{Subject={Subject}, Expectation={Expectation}}}");
    }
}
