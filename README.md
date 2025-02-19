<img src="docs/assets/images/awesomeassertions-banner-responsive.svg" width="40%" />

# A fork of FluentAssertions controlled by the community

[![](https://img.shields.io/github/actions/workflow/status/AwesomeAssertions/AwesomeAssertions/build.yml?branch=main)](https://github.com/AwesomeAssertions/AwesomeAssertions/actions?query=branch%3Amain)
[![Coveralls branch](https://img.shields.io/coverallsCoverage/github/AwesomeAssertions/AwesomeAssertions?branch=main)](https://coveralls.io/github/AwesomeAssertions/AwesomeAssertions?branch=main)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=AwesomeAssertions_AwesomeAssertions&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=AwesomeAssertions_AwesomeAssertions)
[![](https://img.shields.io/github/release/AwesomeAssertions/AwesomeAssertions.svg?label=latest%20release&color=007edf)](https://github.com/AwesomeAssertions/AwesomeAssertions/releases/latest)
[![](https://img.shields.io/nuget/dt/AwesomeAssertions.svg?label=downloads&color=007edf&logo=nuget)](https://www.nuget.org/packages/AwesomeAssertions)
[![](https://img.shields.io/librariesio/dependents/nuget/AwesomeAssertions.svg?label=dependent%20libraries)](https://libraries.io/nuget/AwesomeAssertions)
[![GitHub Repo stars](https://img.shields.io/github/stars/AwesomeAssertions/AwesomeAssertions)](https://github.com/AwesomeAssertions/AwesomeAssertions/stargazers)
[![GitHub contributors](https://img.shields.io/github/contributors/AwesomeAssertions/AwesomeAssertions)](https://github.com/AwesomeAssertions/AwesomeAssertions/graphs/contributors)
[![GitHub last commit](https://img.shields.io/github/last-commit/AwesomeAssertions/AwesomeAssertions)](https://github.com/AwesomeAssertions/AwesomeAssertions)
[![GitHub commit activity](https://img.shields.io/github/commit-activity/m/AwesomeAssertions/AwesomeAssertions)](https://github.com/AwesomeAssertions/AwesomeAssertions/graphs/commit-activity)
[![open issues](https://img.shields.io/github/issues/AwesomeAssertions/AwesomeAssertions)](https://github.com/AwesomeAssertions/AwesomeAssertions/issues)
![](https://img.shields.io/badge/release%20strategy-githubflow-orange.svg)

FluentAssertions, up to version 7.x, was licensed under Apache 2.0. Starting with later versions, licensing changes introduced potential fees for developers.  
In response to these changes, this community project was created. Please read the [FAQ](#FAQ) for more details.

Visit https://awesomeassertions.org for [background information](https://awesomeassertions.org/about/), [usage documentation](https://awesomeassertions.org/introduction), an [extensibility guide](https://awesomeassertions.org/extensibility/), support information and more [tips & tricks](https://awesomeassertions.org/tips/).

# Automatic replacement using Renovate Bot

If you are using Renovate Bot, you can automatically replace FluentAssertions with AwesomeAssertions by adding the
following rule to your `renovate.json`:

```json
{
  "packageRules": [
    {
      "matchPackageNames": ["FluentAssertions"],
      "replacementName": "AwesomeAssertions",
      "replacementVersion": "7.0.0"
    }
  ]
}
```

Renovate will then open PRs to replace FluentAssertions with AwesomeAssertions where applicable.

Alternatively, use PowerShell [script](https://github.com/AwesomeAssertions/AwesomeAssertions/discussions/6) to replace the dependencies.

# How do I build this?
Install Visual Studio 2022 17.8+ or JetBrains Rider 2021.3 as well as the Build Tools 2022 (including the Universal Windows Platform build tools). You will also need to have .NET Framework 4.7 SDK and .NET 8.0 SDK installed. Check the [global.json](global.json) for the current minimum required version.

# What are these Approval.Tests?
This is a special set of tests that use the [Verify](https://github.com/VerifyTests/Verify) project to verify whether you've introduced any breaking changes in the public API of the library.

If you've verified the changes and decided they are valid, you can accept them  using `AcceptApiChanges.ps1` or `AcceptApiChanges.sh`. Alternatively, you can use the [Verify Support](https://plugins.jetbrains.com/plugin/17240-verify-support) plug-in to compare the changes and accept them right from inside Rider. See also the [Contribution Guidelines](CONTRIBUTING.md).

# FAQ

**Q: Who are the maintainers?**  
**A:** The current maintainers of AwesomeAssertions are @cbersch, @jcfnomada, @jupjohn, @IT-VBFK, and @ScarletKuro.

**Q: Will the license change to a more permissive or restrictive license compared to Apache 2.0?**  
**A:** The license will never change, not even to MIT. We will only maintain the original Apache 2.0 license.

**Q: How is it possible that you released version 8 with almost identical changes if version 8 of FluentAssertions is under a commercial license?**  
**A:** This was possible because the license change was made at the final stage of the version 8 release. Any commits made before the license change were free to use, as licenses cannot be applied retroactively. If commits were added to the branch while it was under the Apache 2.0 license, they remain under Apache 2.0. So, any commits before this change [fluentassertions/fluentassertions@df7e9bf](https://github.com/fluentassertions/fluentassertions/commit/df7e9bf8305ef5e26ae58fe4142f8d1b6c4fc4af) can be legally used under the Apache 2.0 terms.

**Q: What is the benefit of this project, and will it continue to evolve and be maintained?**  
**A:** The development of the project depends on the community. We will review and merge pull requests that meet the project's requirements. We actively cherry-pick relevant changes from FluentAssertions version 7 and add them to our fork, as FluentAssertions version 7 is under the old license.  
This project is useful for users who are concerned about potential license issues with version 8 or those working in environments where commercial use could cause licensing complications. Our fork eliminates these concerns and offers a clean solution for such cases.

**Q: Where can I find the documentation?**  
**A:** You can find the documentation at https://awesomeassertions.org

**Q: Why is this package using the FluentAssertions namespace? Isn't that illegal?**  
**A:** The namespaces are part of the API, which was developed under the Apache 2.0 license. The Google v. Oracle case ruled that APIs are considered fair use, so including the 'FluentAssertions' namespace in the API class names is acceptable. While this is permissible now, we may consider changing the namespaces in the future.


# Legal Disclaimer

- This package is not affiliated with or endorsed by the authors of FluentAssertions.
- FluentAssertions is a trademark of its respective owners.
- This package was made possible by the hard work and dedication of the original authors and more than 200 contributors to FluentAssertions. We are extremely grateful for their efforts.
- For the latest information about FluentAssertions, visit the [official repository](https://github.com/fluentassertions/fluentassertions).
