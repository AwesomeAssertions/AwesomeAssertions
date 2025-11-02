### Assorted

* High-level principles such as written down in https://github.com/fluentassertions/fluentassertions/issues/1281#issuecomment-636414698
* Behavioral patterns such as how to deal with `null` arguments
* When to return `AndWhichConstraint` vs `AndConstraint`.
* When to use `FailWith` vs throwing a normal .NET exception type
* Prefer "Did not expect something to be" over "Expected something not to be"
* Decisions (like a poor mans Architecture Design Log) such as why we target certain frameworks
* When using `AndWhichConstraint<..., T>` the `T` element must be fetched in way resilient to that `FailWith` don't throw immediately when encapsulated in a custom `AssertionScope`.
* Assertion class over non-sealed classes or interfaces should probably be generic in type to avoid loosing type information. E.g instead of `MyClassAssertions` then `MyClassAssertions<T> where T : MyClass`
* Be aware that an assertion might be wrapped in an `AssertionScope` so `FailWith` does not halt execution and extra precautions must be taken to avoid e.g. a `NullReferenceException` after verifying that `Subject is not null`
* Don't use `predicate.Body` to format a predicate in a failure message. Just pass the predicate to `FailWith` and let the `PredicateLambdaExpressionValueFormatter` handle formatting it properly
* Don't use `type.Name` to format a type, but rather pass it to `FailWith` and let the `TypeValueFormatter` do its thing.
* Naming and grouping guidelines (based on [this post](https://www.continuousimprover.com/2023/03/test-naming.html))
    * Group tests for the same API using a nested class such as in [NullableNumericSpecs](https://github.com/awesomeassertions/awesomeassertions/blob/main/Tests/FluentAssertions.Specs/Numeric/NullableNumericAssertionSpecs.cs#L210) so you don't have to repeat the API name in the name of the test
    * Avoid the use of `When` and `Should` in test names and use concise names like `Exclusion_of_missing_members_works_with_mapping`
* When writing parameterized tests prefer using `[InlineData]`, unless:
    * the values are not compile-time constants, or
    * there is a good reason to share the parameters between multiple tests.
* Don't use `Should().NotThrow` in the assert.

### Code style

* String formatting should use Invariant culture when formatting non-strings.
* Lines should not be wider than 130 characters.
* Prefer `is null` and `is not null` over `!=/== null`
