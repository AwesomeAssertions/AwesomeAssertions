# Coding style and design guidelines

This document describes the coding style and design guidelines for AwesomeAssertions.
Its content gets detailed from top to bottom.
Start reading the section titles, go in deep if the title is not self-explanatory.

## Code style

* Lines should not be wider than 130 characters.
* Prefer `is null` and `is not null` over `!=/== null`.
* String formatting should use invariant culture when formatting non-strings.
* Follow the style presented in the [Coding Guidelines for C#](https://csharpcodingguidelines.com/).

## Design guidelines

### General

### Core assembly

* Pay attention to backwards compatibility.
* Keep the code base open for extensibility.
* Mind the API fluent design. (Test should be readable like natural human language.)
* Design the features discoverable.

* ✅ Prefer "Did not expect something to be [...]" over "Expected something not to be [...]".
* ✅ Be aware that an assertion might be wrapped in an `AssertionScope` so `FailWith` does not halt execution
  and extra precautions must be taken to avoid e.g. a `NullReferenceException` after verifying that `Subject is not null`.

* ❌ Don't use `predicate.Body` to format a predicate in a failure message.
  Pass the predicate to `FailWith` and let the `PredicateLambdaExpressionValueFormatter` handle formatting it properly.
* ❌ Don't use `type.Name` to format a type, but rather pass it to `FailWith` and let the `TypeValueFormatter` do its thing.

### Tests in AwesomeAssertions

* Naming and grouping guidelines (based on [this post](https://www.continuousimprover.com/2023/03/test-naming.html))
  * ✅ Group tests for the same API using a nested class so you don't have to repeat the API name in the name of the test.
  * ❌ Avoid the use of `When` and `Should` in test names and use concise names like `Exclusion_of_missing_members_works_with_mapping`.
* Every test method shall follow the AAA rule: Arrange, Act, Assert.
  * ✅ Separate Arrange, Act and Assert with exactly one empty line.
  * ❌ Additional comments for these blocks are not required.
* Remember to test the "because formatting" overloads.
  * ✅ Always use the pattern `"we want to test the {0} message", "failure"`
    resulting in generated string `"because we want to test the failure message"`.
    At least the part `"because*failure message"` should be tested.
* ❌ Don't use `Should().NotThrow` in the asserting for tests which are meant to pass.

### TODO - unsorted

* Behavioral patterns such as how to deal with `null` arguments
* When to return `AndWhichConstraint` vs `AndConstraint`.
* When to use `FailWith` vs throwing a normal .NET exception type
* Decisions (like a poor mans Architecture Design Log) such as why we target certain frameworks
* When using `AndWhichConstraint<..., T>` the `T` element must be fetched in way resilient to that `FailWith` don't throw immediately when encapsulated in a custom `AssertionScope`.
* Assertion class over non-sealed classes or interfaces should probably be generic in type to avoid loosing type information. E.g instead of `MyClassAssertions` then `MyClassAssertions<T> where T : MyClass`
