﻿using System;

namespace AwesomeAssertions;

/// <summary>
/// Marks a method as an extension to Awesome Assertions that either uses the built-in assertions
/// internally, or directly uses <c>AssertionChain</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
#pragma warning disable CA1813 // Avoid unsealed attributes. This type has shipped.
public class CustomAssertionAttribute : Attribute;
