---
title: Assertion Scopes
permalink: /assertionscopes/
layout: single
classes: wide
sidebar:
  nav: "sidebar"
---

You can batch multiple assertions into an `AssertionScope` so that Awesome Assertions throws one exception at the end of the scope with all failures.

E.g.

```csharp
using (new AssertionScope())
{
    5.Should().Be(10);
    "Actual".Should().Be("Expected");
}
```

The above will batch the two failures, and throw an exception at the point of disposing the `AssertionScope` displaying both errors.

E.g. Exception thrown at point of dispose contains:

```text
Expected value to be 10, but found 5 (difference of -5).
Expected string to be "Expected" with a length of 8, but "Actual" has a length of 6, differs near "Act" (index 0).
```

## Nesting

You can also nest two of those scopes and give them suitable names:

```csharp
using var outerScope = new AssertionScope("Test1");
using var innerScope = new AssertionScope("Test2");
nonEmptyList.Should().BeEmpty();
```

This will give you:

      Expected Test1/Test2/nonEmptyList to be empty, but found at least one item {1}.

In more sophisticated scenarios, you might want to intercept the assertion raised within an `AssertionScope` and prevent it from throwing an exception.

```csharp
using (var scope = new AssertionScope())
{
    5.Should().Be(10);
    // other assertion left out for brevity...

    // Collect all the failure messages that occurred up to this point
    string[] failures = scope.Discard();

    // The closing brace will not throw any exceptions anymore
}
```

## Reportables

You can add custom information to the current assertion scope with `AssertionScope.AddReportable`. When the scope fails, the reportable data is appended to the failure message.

Use the overloads that match your scenario:

- `AddReportable(string key, string value)` adds a plain string value.
- `AddReportable(string key, Func<string> getValue)` defers value calculation until the scope fails.
- `AddReportable(string key, object value)` adds an object that is formatted using the scope’s configured formatter.

```csharp
using var scope = new AssertionScope();

scope.AddReportable("UserId", 123);
scope.AddReportable("RequestId", () => Guid.NewGuid().ToString());
scope.AddReportable("Description", "Investigating why the list was not empty");

new[] { 1, 2, 3 }.Should().BeEmpty();
```

When working with `AssertionChain` reportables are added with `AssertionChain.WithReportable` to the current scope.

## Scoped `IValueFormatter`s

You can add a custom value formatter inside a scope to selectively customize formatting of an object based on the context of the test.
To achieve that, you can do following:

```csharp
using var scope = new AssertionScope();

var formatter = new CustomFormatter();
scope.FormattingOptions.AddFormatter(formatter);
```

You can even add formatters to nested assertion scopes and the nested scope will pick up all previously defined formatters:

```csharp
using var outerScope = new AssertionScope();

var outerFormatter = new OuterFormatter();
var innerFormatter = new InnerFormatter();
outerScope.FormattingOptions.AddFormatter(outerFormatter);

using var innerScope = new AssertionScope();
innerScope.FormattingOptions.AddFormatter(innerFormatter);

// At this point outerFormatter and innerFormatter will be available
```

**Note:** If you modify the scoped formatters inside the nested scope, it won't touch the scoped formatters from the outer scope:

```csharp
using var outerScope = new AssertionScope();

var outerFormatter = new OuterFormatter();
var innerFormatter = new InnerFormatter();
outerScope.FormattingOptions.AddFormatter(outerFormatter);

using (var innerScope = new AssertionScope())
{
  innerScope.FormattingOptions.AddFormatter(innerFormatter);
  innerScope.FormattingOptions.RemoveFormatter(outerFormatter);

  // innerScope only contains innerFormatter
}

// outerScope still contains outerFormatter
```

## `AssertionScope` vs `AssertionChain`

The `AssertionScope` targets users which want to group existing assertions to get all possible error messages and not only the first one.

The `AssertionChain` is the entrance point into the internal fluent API. It simplifies writing assertions and carries context information if an assertion is composed of multiple conditions. For more details on extensibility see [extensibility.md].