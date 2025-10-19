---
title: About
permalink: /about/
layout: single
toc: true
sidebar:
  nav: "sidebar"
---

## Why?

Nothing is more annoying than a unit test that fails without clearly explaining why. More than often, you need to set a breakpoint and start up the debugger to be able to figure out what went wrong. Jeremy D. Miller once gave the advice to "keep out of the debugger hell" and I can only agree with that.

For instance, only test a single condition per test case. If you don't, and the first condition fails, the test engine will not even try to test the other conditions. But if any of the others fail, you'll be on your own to figure out which one. I often run into this problem when developers try to combine multiple related tests that test a member using different parameters into one test case. If you really need to do that, consider using a parameterized test that is being called by several clearly named test cases.

That’s why we designed Awesome Assertions to help you in this area. Not only by using clearly named assertion methods, but also by making sure the failure message provides as much information as possible. Consider this example:

```csharp
string accountNumber = "1234567890";
accountNumber.Should().Be("0987654321");
```

This will be reported as:

> Expected accountNumber to be
"0987654321", but
"1234567890" differs near "123" (index 0).

The fact that both strings are displayed on a separate line is not a coincidence and happens if any of them is longer than 8 characters. However, if that's not enough, all assertion methods take an optional explanation (the because) that supports formatting placeholders similar to String.Format which you can use to enrich the failure message. For instance, the assertion

```csharp
var numbers = new[] { 1, 2, 3 };
numbers.Should().Contain(item => item > 3, "at least {0} item should be larger than 3", 1);
```

will fail with:

> Expected numbers to have an item matching (item > 3) because at least 1 item should be larger than 3.

## Supported Frameworks and Libraries

Awesome Assertions cross-compiles to .NET Framework 4.7, as well as .NET 6, .NET Standard 2.0 and 2.1.

Because of that Awesome Assertions supports the following minimum platforms.

* .NET Framework 4.7 and later
* .NET 6.0 and later
* Mono 5.4, Xamarin.iOS 10.14, Xamarin.Mac 3.8 and Xamarin.Android 8.0
* Universal Windows Platform 10.0.16299 and later

Awesome Assertions supports the following unit test frameworks:

* [MSTest V2, V3, V4](https://github.com/Microsoft/testfx) (Visual Studio 2019, Visual Studio 2022)
* [NUnit2, NUnit3](http://www.nunit.org/)
* [xUnit2, xUnit3](https://github.com/xunit/xunit/releases)
* [MSpec](https://github.com/machine/machine.specifications)
* [TUnit](https://github.com/thomhurst/TUnit)

## Coding by Example

As you may have noticed, the purpose of this open-source project is to not only be the best assertion framework in the .NET realm, but to also demonstrate high-quality code.
We heavily practice Test Driven Development and one of the promises TDD makes is that unit tests can be treated as your API's documentation.
So although you are free to go through the many examples here, please consider to analyze the many [unit tests](https://github.com/awesomeassertions/awesomeassertions/tree/main/Tests/AwesomeAssertions.Specs).

## Who is behind this project

Originally developed as Fluent Assertions by Dennis Doomen and Jonas Nyrup, the Awesome Assertions fork is continued by the [AwesomeAssertions team](https://github.com/orgs/AwesomeAssertions/people) under the [original, free Apache-2 license](https://github.com/AwesomeAssertions/AwesomeAssertions/blob/main/LICENSE).

## Versioning

The version numbers of Awesome Assertions releases comply to the [Semantic Versioning](http://semver.org/) scheme. In other words, release 1.4.0 only adds backwards-compatible functionality and bug fixes compared to 1.3.0. Release 1.4.1 should only include bug fixes. And if we ever introduce breaking changes, the number increased to 2.0.0.
