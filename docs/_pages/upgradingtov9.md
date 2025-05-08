---
title: Upgrading to version 9.0
permalink: /upgradingtov9
layout: single
toc: true
sidebar:
  nav: "sidebar"
---

## Changing all `FluentAssertions` namings to `AwesomeAssertions`

As of v9, we've decided to change all `FluentAssertions` namings to `AwesomeAssertions`. This affects namespaces as well as assembly, project and solution names.

There are no functional changes in this release on purpose to give users more time to migrate without missing any new features and fixes.

## Upgrading

### Replacing namespaces

When upgrading to v9 it should be sufficient in most cases to simply replace all occurrences of `FluentAssertions` with `AwesomeAssertions`.

Unfortunately, `global using` cannot be use to map between the namespaces, because [no `using` alias can be used in the declaration of a `using` directive](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/using-directive#the-using-alias).