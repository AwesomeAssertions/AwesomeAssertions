# Coding style and design guidelines

Guidelines for contributing to AwesomeAssertions, ordered so you can skim.
Read the **bold rules** and the ✅ / ❌ lines;
dig into a section only when its title isn't self-explanatory.

## Code style

* Lines ≤ 130 characters.
* Prefer `is null` / `is not null` over `== null` / `!= null`.
* Use invariant culture when formatting non-strings.
* Otherwise follow the [C# Coding Guidelines](https://csharpcodingguidelines.com/).

## Design principles

What every public API optimises for:

* **Backwards compatibility** — the public surface is a contract (see *Evolving the public API*).
* **Reads like a sentence** — the fluent chain should form natural English.
* **Discoverable** — the right method should be findable through IntelliSense, without the docs.
* **Extensible** — keep the hooks (`Using`, custom rules / steps / comparers) open for third parties.
* ✅ Make assertion classes for non-sealed types or interfaces **generic in the subject type**,
  so chaining keeps the concrete type —
  `MyTypeAssertions<TSubject, TAssertions> where TSubject : MyType`,
  mirroring the built-in `NumericAssertions<T, TAssertions>` —
  not a non-generic `MyTypeAssertions`.

## API consistency

A fluent assertion should read like a sentence and be discoverable.
Two rules keep it that way:

1. **Name by what the method *does* (observable behaviour) — not by a matching prefix or its implementation.**
2. **Reuse a category; a method and its opposite live in the *same* one** (`Including` ↔ `Excluding`).

A **new category is a last resort**, justified only by a genuinely new *mechanism* — never by "the prefix fits".
Prefer a precise gerund with a clear, opposite-friendly meaning.

**Register by role:** fluent builders chain, use gerund / `With…` names and return the builder;
collection & settings objects mutate and use imperative `Add` / `Remove` / `Clear` or settable properties.

| Category | Answers | Example |
|---|---|---|
| `Including` / `Excluding` | *which* members participate | `ExcludingFields()` |
| `Ignoring` | *how* a participating value is compared | `IgnoringCase()` |
| `Comparing` | which equality algorithm for a type | `ComparingByValue<T>()` |
| `Preferring` | declared vs. runtime type view | `PreferringRuntimeMemberTypes()` |
| `Allowing` | lifts a framework guard | `AllowingInfiniteRecursion()` |
| `Suppressing` | swallows a third-party exception | `SuppressingEventAccessorExceptions()` |
| `Treating…As…` | reclassifies member status | `Treating…AsMissing()` |
| `With` / `Without` | toggles a feature / mode | `WithStrictOrdering()` |
| `Using` | plugs in a custom rule / step / comparer | `Using(IMemberSelectionRule)` |

Assertion methods use the `Be*` / `Have*` / `Contain*` families, negating with `Not` + positive;
a precise bare domain verb (`Throw`, `Match`, `Imply`) is fine where a family prefix would read worse.

## Writing an assertion

* ✅ **Fail, don't throw, for assertion outcomes.** Report every verdict about the *subject* —
  including a **null subject** — through `FailWith`.
  Throw a real exception only for **API misuse**:
  use `Guard.ThrowIfArgumentIsNull` for arguments that must never be null (a predicate, options, a comparer).
* ✅ **Assume an `AssertionScope`.** Inside a scope, `FailWith` **collects** the failure instead of throwing,
  so execution continues.
  Guard against a follow-up `NullReferenceException` (e.g. after `Subject is not null`),
  and when returning `AndWhichConstraint<…, T>`,
  fetch the `T` **defensively** — a prior assertion may have failed without throwing.
* ✅ **Pick the continuation deliberately:** return `AndConstraint<T>` to chain more assertions with `.And`;
  return `AndWhichConstraint<T, S>` only when the assertion selects a single subject `S`
  the caller is likely to continue on (`.Which` / `.Subject`).
* ✅ Prefer *"Did not expect … to be […]"* over *"Expected … not to be […]"* in failure messages.
* ❌ Don't format a predicate via `predicate.Body` — pass the predicate to `FailWith` and let
  `PredicateLambdaExpressionValueFormatter` render it.
* ❌ Don't format a type via `type.Name` — pass the type to `FailWith` and let `TypeValueFormatter` do it.

## Evolving the public API

When a member is to be replaced, renamed or removed in a future version:

* ✅ Extend the member's XML `<summary>` to point at the replacement.
* ✅ Add a `Deprecations` note to the release notes (after the `Fixes` section, within the version).
* ✅ Hide the old member with `[EditorBrowsable(EditorBrowsableState.Never)]`.
* ❌ Don't use `[Obsolete]` — many users build with "warnings as errors", so it would break their build.

## Tests

Naming and grouping (based on [this post](https://www.continuousimprover.com/2023/03/test-naming.html)):

* ✅ Group tests for one API in a nested class, so the API name isn't repeated in each test name.
* ✅ Use concise names like `Exclusion_of_missing_members_works_with_mapping`.
* ❌ Avoid `When` and `Should` in test names.
* ✅ Follow **A**rrange – **A**ct – **A**ssert, separated by exactly one blank line.
* ❌ Omit AAA comments unless they are genuinely meaningful.
* ✅ Cover the "because" overload with the pattern `"we want to test the {0} message", "failure"`
  and assert on the full text or `"*because*failure message*"`.
* ❌ Don't use `Should().NotThrow(...)` as the *act* of a test meant to pass — assert the outcome directly.
