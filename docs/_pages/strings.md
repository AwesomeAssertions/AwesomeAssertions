---
title: Strings
permalink: /strings/
layout: single
classes: wide
sidebar:
  nav: "sidebar"
---

For asserting whether a string is null, empty, contains whitespace only, or is in upper/lower case, you have a wide range of methods to your disposal.

```csharp
string theString = "";
theString.Should().NotBeNull();
theString.Should().BeNull();
theString.Should().BeEmpty();
theString.Should().NotBeEmpty("because the string is not empty");
theString.Should().HaveLength(0);
theString.Should().BeNullOrWhiteSpace(); // either null, empty or whitespace only
theString.Should().NotBeNullOrWhiteSpace();
```

To ensure that the characters with case in a string are all upper or lower cased (or the opposite), you can use the following assertions.

```csharp
theString.Should().BeUpperCased();
theString.Should().NotBeUpperCased();
theString.Should().BeLowerCased();
theString.Should().NotBeLowerCased();
```

Note that numbers, special characters, and some alphabets don't have casing, so `BeUpperCased` and `BeLowerCased` will
ignore these characters. In other words, `BeUpperCased` will succeed if the string is a possible output of
`ToUpperInvariant`, and likewise for `BeLowerCased`. `NotBeUpperCased` will fail only if the string contains characters
with case, and all those characters are upper-case, and likewise for `NotBeLowerCased`.

The semantics of these assertions changed in Awesome Assertions v7.0. For the previous semantics, asserting that all
characters in the string are upper-case characters, for example, you can use a collection assertion on the characters of
the string:

```csharp
theString.Should().OnlyContain(char.IsUpper);
```

Obviously you'll find all the methods you would expect for string assertions.

```csharp
theString = "This is a String";
theString.Should().Be("This is a String");
theString.Should().NotBe("This is another String");
theString.Should().BeEquivalentTo("THIS IS A STRING");
theString.Should().NotBeEquivalentTo("THIS IS ANOTHER STRING");

theString.Should().BeOneOf(
    "That is a String",
    "This is a String",
);

theString.Should().Contain("is a");
theString.Should().Contain("is a", Exactly.Once());
theString.Should().Contain("is a", AtLeast.Twice());
theString.Should().Contain("is a", MoreThan.Thrice());
theString.Should().Contain("is a", AtMost.Times(5));
theString.Should().Contain("is a", LessThan.Twice());
theString.Should().ContainAll("should", "contain", "all", "of", "these");
theString.Should().ContainAny("any", "of", "these", "will", "do");
theString.Should().NotContain("is a");
theString.Should().NotContainAll("can", "contain", "some", "but", "not", "all");
theString.Should().NotContainAny("can't", "contain", "any", "of", "these");
theString.Should().ContainEquivalentOf("WE DONT CARE ABOUT THE CASING");
theString.Should().ContainEquivalentOf("WE DONT CARE ABOUT THE CASING", Exactly.Once());
theString.Should().ContainEquivalentOf("WE DONT CARE ABOUT THE CASING", AtLeast.Twice());
theString.Should().ContainEquivalentOf("WE DONT CARE ABOUT THE CASING", MoreThan.Thrice());
theString.Should().ContainEquivalentOf("WE DONT CARE ABOUT THE CASING", AtMost.Times(5));
theString.Should().ContainEquivalentOf("WE DONT CARE ABOUT THE CASING", LessThan.Twice());
theString.Should().NotContainEquivalentOf("HeRe ThE CaSiNg Is IgNoReD As WeLl");

theString.Should().StartWith("This");
theString.Should().NotStartWith("This");
theString.Should().StartWithEquivalentOf("this");
theString.Should().NotStartWithEquivalentOf("this");

theString.Should().EndWith("a String");
theString.Should().NotEndWith("a String");
theString.Should().EndWithEquivalentOf("a string");
theString.Should().NotEndWithEquivalentOf("a string");
```

All equivalency methods which end with "EquivalentOf" can be fine-tuned in its behavior what differences to ignore.
For instance, if you want to ignore leading whitespace, use this:

```csharp
theString.Should().BeEquivalentTo("This is a string", o => o.IgnoringLeadingWhitespace());
```

The supported options are:

| Option                       | Behavior                                                                                          |
| ---------------------------- | ------------------------------------------------------------------------------------------------- |
| `IgnoringLeadingWhitespace`  | Ignores leading whitespace in the subject and the expectation.                                    |
| `IgnoringTrailingWhitespace` | Ignores trailing whitespace in the subject and the expectation.                                   |
| `IgnoringCase`               | Compares the strings case-insensitive.                                                            |
| `IgnoringNewlineStyle`       | Replaces `"\r\n"` and `"\r"` with `"\n"` before comparing the subject and expectation.            |

You can also specify a custom string comparer via
```csharp
theString.Should().BeEquivalentTo("THIS IS A STRING", o => o.Using(StringComparer.OrdinalIgnoreCase));
```

For the `Match`, `NotMatch`, `MatchEquivalentOf`, and `NotMatchEquivalentOf` methods we support wildcards.

The pattern can be a combination of literal and wildcard characters, but it doesn't support regular expressions.

The following wildcard specifiers are permitted in the pattern:

| Wildcard specifier | Matches                                   |
| ----------------- | ----------------------------------------- |
| * (asterisk)      | Zero or more characters in that position. |
| ? (question mark) | Exactly one character in that position.   |

For instance, if you would like to assert that some email address is correct, use this:

```csharp
emailAddress.Should().Match("*@*.com");
homeAddress.Should().NotMatch("*@*.com");
```

If the casing of the input string is irrelevant, use this:

```csharp
emailAddress.Should().MatchEquivalentOf("*@*.COM");
emailAddress.Should().NotMatchEquivalentOf("*@*.COM");
```

And if wildcards aren't enough for you, you can always use some regular expression magic:

```csharp
someString.Should().MatchRegex("h.*\\sworld.$");
someString.Should().MatchRegex(new Regex("h.*\\sworld.$"));
subject.Should().NotMatchRegex(new Regex(".*earth.*"));
subject.Should().NotMatchRegex(".*earth.*");
```

And if that's not enough, you can assert on the number of matches of a regular expression:

```csharp
someString.Should().MatchRegex("h.*\\sworld.$", Exactly.Once());
someString.Should().MatchRegex(new Regex("h.*\\sworld.$"), AtLeast.Twice());
```

If you prefer a more fluent syntax than `Exactly.Times(4)`, `AtLeast.Times(4)` and `AtMost.Times(4)` reads, you can do the following:

```csharp
theString.Should().Contain("is a", 4.TimesExactly()); // equivalent to Exactly.Times(4)
theString.Should().Contain("is a", 4.TimesOrMore());  // equivalent to AtLeast.Times(4)
theString.Should().Contain("is a", 4.TimesOrLess());  // equivalent to AtMost.Times(4)
```