using System;

namespace FluentAssertions;

/// <summary>
/// Marks an assembly as containing extensions to Awesome Assertions that either uses the built-in assertions
/// internally, or directly uses <c>AssertionChain</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class CustomAssertionsAssemblyAttribute : Attribute;
