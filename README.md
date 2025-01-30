# A fork of FluentAssertions controlled by the community

[![](https://img.shields.io/github/actions/workflow/status/AwesomeAssertions/AwesomeAssertions/build.yml?branch=main)](https://github.com/AwesomeAssertions/AwesomeAssertions/actions?query=branch%3Amain)
[![Coveralls branch](https://img.shields.io/coverallsCoverage/github/AwesomeAssertions/AwesomeAssertions?branch=main)](https://coveralls.io/github/AwesomeAssertions/AwesomeAssertions?branch=main)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=AwesomeAssertions_AwesomeAssertions&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=AwesomeAssertions_AwesomeAssertions)
[![](https://img.shields.io/github/release/AwesomeAssertions/AwesomeAssertions.svg?label=latest%20release&color=007edf)](https://github.com/AwesomeAssertions/AwesomeAssertions/releases/latest)
[![](https://img.shields.io/nuget/dt/AwesomeAssertions.svg?label=downloads&color=007edf&logo=nuget)](https://www.nuget.org/packages/AwesomeAssertions)
[![](https://img.shields.io/librariesio/dependents/nuget/AwesomeAssertions.svg?label=dependent%20libraries)](https://libraries.io/nuget/AwesomeAssertions)
[![GitHub Repo stars](https://img.shields.io/github/stars/AwesomeAssertions/AwesomeAssertions)](https://github.com/AwesomeAssertions/AwesomeAssertions/stargazers)
[![GitHub contributors](https://img.shields.io/github/contributors/AwesomeAssertions/AwesomeAssertions)](https://github.com/fluentassertions/AwesomeAssertions/graphs/contributors)
[![GitHub last commit](https://img.shields.io/github/last-commit/AwesomeAssertions/AwesomeAssertions)](https://github.com/AwesomeAssertions/AwesomeAssertions)
[![GitHub commit activity](https://img.shields.io/github/commit-activity/m/AwesomeAssertions/AwesomeAssertions)](https://github.com/AwesomeAssertions/AwesomeAssertions/graphs/commit-activity)
[![open issues](https://img.shields.io/github/issues/AwesomeAssertions/AwesomeAssertions)](https://github.com/AwesomeAssertions/AwesomeAssertions/issues)
![](https://img.shields.io/badge/release%20strategy-githubflow-orange.svg)


## By the community, for the community

I, @meenzen hereby commit to keeping the Apache 2 license of AwesomeAssertions intact so that you may depend on in as a
drop-in replacement without having to fear any license violations by accidentally upgrading.

## Maintenance

Please do not expect updates to this fork, as I cannot commit to maintaining it in the future. I will do my best to
review and merge critical PRs if required and to release new versions of AwesomeAssertions as needed.

## Automatic replacement using Renovate Bot

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

## Acknowledgements

This was made possible by the hard work and dedication of the original authors and more than 200 contributors of
FluentAssertions. We are extremely grateful for their efforts.
