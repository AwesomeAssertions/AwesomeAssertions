using System;

namespace FluentAssertions;

/// <summary>
/// Marks a class as containing extensions to Awesome Assertions that either uses the built-in assertions
/// internally, or directly uses <c>AssertionChain</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class CustomAssertionsAttribute : Attribute;
