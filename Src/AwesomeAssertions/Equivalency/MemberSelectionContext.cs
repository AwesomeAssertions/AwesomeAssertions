using System;
using AwesomeAssertions.Common;

namespace AwesomeAssertions.Equivalency;

/// <summary>
/// Provides contextual information to an <see cref="IMemberSelectionRule"/>.
/// </summary>
public class MemberSelectionContext
{
    private readonly Type compileTimeType;
    private readonly Type runtimeType;
    private readonly IEquivalencyOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemberSelectionContext"/> class.
    /// </summary>
    /// <param name="compileTimeType">The declared (compile-time) type of the object whose members are being selected.</param>
    /// <param name="runtimeType">The run-time type of the object whose members are being selected.</param>
    /// <param name="options">The options that control how the structural equivalency is asserted.</param>
    public MemberSelectionContext(Type compileTimeType, Type runtimeType, IEquivalencyOptions options)
    {
        this.runtimeType = runtimeType;
        this.compileTimeType = compileTimeType;
        this.options = options;
    }

    /// <summary>
    /// Gets a value indicating whether and which properties should be considered.
    /// </summary>
    public MemberVisibility IncludedProperties => options.IncludedProperties;

    /// <summary>
    /// Gets a value indicating whether and which fields should be considered.
    /// </summary>
    public MemberVisibility IncludedFields => options.IncludedFields;

    /// <summary>
    /// Gets either the compile-time or run-time type depending on the options provided by the caller.
    /// </summary>
    public Type Type
    {
        get
        {
            Type type = options.UseRuntimeTyping ? runtimeType : compileTimeType;

            return type.NullableOrActualType();
        }
    }
}
