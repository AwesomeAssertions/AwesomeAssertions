---
title: Introduction
permalink: /introduction
layout: single
classes: wide
redirect_from:
  - /examples
  - /documentation
  - /documentation/
  - /documentation.html
sidebar:
  nav: "sidebar"
---

## Getting started

Awesome Assertions is a set of .NET extension methods that allow you to more naturally specify the expected outcome of a TDD or BDD-style unit test. This enables a simple intuitive syntax that all starts with the following `using` statement:

```csharp
using AwesomeAssertions;
```

This brings a lot of extension methods into the current scope. For example, to verify that a string begins, ends and contains a particular phrase.

```csharp
string actual = "ABCDEFGHI";
actual.Should().StartWith("AB").And.EndWith("HI").And.Contain("EF").And.HaveLength(9);
```

To verify that all elements of a collection match a predicate and that it contains a specified number of elements.

```csharp
IEnumerable<int> numbers = new[] { 1, 2, 3 };

numbers.Should().OnlyContain(n => n > 0);
numbers.Should().HaveCount(4, "because we thought we put four items in the collection");
```

The nice thing about the second failing example is that it will throw an exception with the message (and notice that it uses the name of the variable `numbers`):

    Expected numbers to contain 4 item(s) because we thought we put four items in the collection, but found 3.

To verify that a particular business rule is enforced using exceptions:

```csharp
var recipe = new RecipeBuilder()
  .With(new IngredientBuilder().For("Milk").WithQuantity(200, Unit.Milliliters))
  .Build();

var action = () => recipe.AddIngredient("Milk", 100, Unit.Spoon);

action
  .Should().Throw<RuleViolationException>()
  .WithMessage("*change the unit of an existing ingredient*")
  .And.Violations.Should().Contain(BusinessRule.CannotChangeIngredientQuantity);
```

One neat feature is the ability to chain a specific assertion on top of an assertion that acts on a collection or graph of objects.

```csharp
dictionary.Should().ContainValue(myClass).Which.SomeProperty.Should().BeGreaterThan(0);

someObject.Should().BeOfType<Exception>().Which.Message.Should().Be("Other Message");

xDocument.Should().HaveElement("child").Which.Should().BeOfType<XElement>().And.HaveAttribute("attr", "1");
```

If the first one fails, you will get a message like:

    Expected dictionary["key"].SomeProperty to be greater than 0, but found -2

This chaining can make your unit tests a lot easier to read.

## Global Configurations

Awesome Assertions' `AssertionConfiguration` has several methods and properties that can be used to change the way it executes assertions or the defaults it will use for comparing object graphs. Changing those settings at the right time can be difficult, depending on the test framework. That's why Awesome Assertions offers a special assembly-level attribute that can be used to have some code executed _before_ the first assertion is executed. It will be called only once per test run, but you can use the attribute multiple times.

```csharp
[assembly: AssertionEngineInitializer(typeof(Initializer), nameof(Initializer.Initialize))]

public static class Initializer
{
    public static void Initialize()
    {
        AssertionConfiguration.Current.Equivalency.Modify(options => options
          .ComparingByValue<DirectoryInfo>());
    }
}
```

## Detecting Test Frameworks

Awesome Assertions supports a lot of different unit testing frameworks. Just add a reference to the corresponding test framework assembly to the unit test project. Awesome Assertions will automatically find the corresponding assembly and use it for throwing the framework-specific exceptions.

If, for some unknown reason, Awesome Assertions fails to find the assembly, and you're running under .NET 4.7 or a .NET 8.0 project, try specifying the framework explicitly using a configuration setting in the project’s app.config. If it cannot find any of the supported frameworks, it will fall back to using a custom `AssertionFailedException` exception class.

```xml
<configuration>
  <appSettings>
    <!-- Supported values: nunit, xunit2, xunit3, mstest, mstestv4, mspec and tunit -->
    <add key="AwesomeAssertions.TestFramework" value="nunit"/>
  </appSettings>
</configuration>
```

Just add NuGet package "AwesomeAssertions" to your test project.

## Subject Identification

Awesome Assertions can use the C# code of the unit test to extract the name of the subject and use that in the assertion failure. Consider for instance this statement:

```csharp
string username = "dennis";
username.Should().Be("jonas");
```

This will throw a test framework-specific exception with the following message:

    Expected username to be "jonas" with a length of 5, but "dennis" has a length of 6, differs near "den" (index 0).

The way this works is that Awesome Assertions will try to traverse the current stack trace to find the line and column numbers as well as the full path to the source file. Since it needs the debug symbols for that, this will require you to compile the unit test projects in debug mode, even on your build servers.
Also, this does not work with [`PathMap`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/advanced#pathmap) for unit test projects as it assumes that source files are present on the path returned from [`StackFrame.GetFileName()`](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.stackframe.getfilename).

Now, if you've built your own extensions that use Awesome Assertions directly, you can tell it to skip that extension code while traversing the stack trace. Consider for example the customer assertion:

```csharp
    public class CustomerAssertions
    {
        private readonly Customer customer;

        public CustomerAssertions(Customer customer)
        {
            this.customer = customer;
        }

        [CustomAssertion]
        public void BeActive(string because = "", params object[] becauseArgs)
        {
            customer.Active.Should().BeTrue(because, becauseArgs);
        }
    }
```

And it's usage:

```csharp
myClient.Should().BeActive("because we don't work with old clients");
```

Without the `[CustomAssertion]` attribute, Awesome Assertions would find the line that calls `Should().BeTrue()` and treat the `customer` variable as the subject-under-test (SUT). But by applying this attribute, it will ignore this invocation and instead find the SUT by looking for a call to `Should().BeActive()` and use the `myClient` variable instead.

For whole assertions classes you can use the `[CustomAssertions]` attribute to mark them as being extensions to Awesome Assertions.

Alternatively, you can add the `[assembly:CustomAssertionsAssembly]` attribute to a file within the project to tell Awesome Assertions that all code in that assembly should be treated as custom assertion code.

## Assertion Scopes

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

For details see [Assertion Scopes](assertionscopes.md).
