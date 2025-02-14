<!-- Please provide a description of your changes above the IMPORTANT checklist -->


## IMPORTANT 

* [ ] If the PR touches the public API, the changes have been approved in a separate issue with the "api-approved" label.
* [ ] The code complies with the [Coding Guidelines for C#](https://www.csharpcodingguidelines.com/).
* [ ] The changes are covered by unit tests which follow the Arrange-Act-Assert syntax and the naming conventions such as is used [in these tests](../tree/main/Tests/FluentAssertions.Equivalency.Specs/MemberMatchingSpecs.cs#L51-L430).
* [ ] If the PR adds a feature or fixes a bug, please update [the release notes](../tree/main/docs/_pages/releases.md) with a functional description that explains what the change means to consumers of this library, which are published on the [website](https://awesomeassertions.github.io/AwesomeAssertions/releases).
* [ ] If the PR changes the public API the changes needs to be included by running [AcceptApiChanges.ps1](../tree/main/AcceptApiChanges.ps1) or [AcceptApiChanges.sh](../tree/main/AcceptApiChanges.sh).
* [ ] If the PR affects [the documentation](../tree/main/docs/_pages), please include your changes in this pull request so the documentation will appear on the [website](https://awesomeassertions.github.io/AwesomeAssertions/introduction).
    * [ ] Please also run `./build.sh --target spellcheck` or `.\build.ps1 --target spellcheck` before pushing and check the good outcome
