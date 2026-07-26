namespace AwesomeAssertions.Common;

/// <summary>
/// Represents the access modifier of a C# member or type.
/// </summary>
public enum CSharpAccessModifier
{
    /// <summary>
    /// The member or type is accessible from anywhere (<c>public</c>).
    /// </summary>
    Public,

    /// <summary>
    /// The member or type is only accessible within its containing type (<c>private</c>).
    /// </summary>
    Private,

    /// <summary>
    /// The member or type is accessible within its containing type and by derived types (<c>protected</c>).
    /// </summary>
    Protected,

    /// <summary>
    /// The member or type is accessible only within the same assembly (<c>internal</c>).
    /// </summary>
    Internal,

    /// <summary>
    /// The member or type is accessible within the same assembly or by derived types (<c>protected internal</c>).
    /// </summary>
    ProtectedInternal,

    /// <summary>
    /// The combination of access modifiers does not map to a valid C# access modifier keyword.
    /// </summary>
    InvalidForCSharp,

    /// <summary>
    /// The member or type is accessible by derived types within the same assembly (<c>private protected</c>).
    /// </summary>
    PrivateProtected,
}
