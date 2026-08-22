using System;
using System.Linq;
using LibGit2Sharp;
using Fallout.Common;
using Fallout.Common.CI.GitHubActions;
using Fallout.Common.Execution;
using Fallout.Common.Git;
using Fallout.Common.IO;
using Fallout.Common.Tooling;
using Fallout.Common.Tools.DotNet;
using Fallout.Common.Tools.GitVersion;
using Fallout.Common.Tools.ReportGenerator;
using Fallout.Common.Tools.Xunit;
using Fallout.Common.Utilities;
using Fallout.Common.Utilities.Collections;
using Fallout.Solutions;
using static Fallout.Common.Tools.DotNet.DotNetTasks;
using static Fallout.Common.Tools.ReportGenerator.ReportGeneratorTasks;
using static Fallout.Common.Tools.Xunit.XunitTasks;
using static Serilog.Log;
using static CustomNpmTasks;

[UnsetVisualStudioEnvironmentVariables]
[DotNetVerbosityMapping]
class Build : FalloutBuild
{
    const string NetFrameworkVersion = "net472";

    public static int Main() => Execute<Build>(x => x.SpellCheck, x => x.Push);

    GitHubActions GitHubActions => GitHubActions.Instance;

    string BranchSpec => GitHubActions?.Ref;

    string BuildNumber => GitHubActions?.RunNumber.ToString();

    string PullRequestBase => GitHubActions?.BaseRef;

    [Parameter("The solution configuration to build. Default is 'Debug' (local) or 'CI' (server).")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.CI;

    [Parameter("Use this parameter if you encounter build problems in any way, " +
        "to generate a .binlog file which holds some useful information.")]
    readonly bool? GenerateBinLog;

    [Parameter("The key to push to Nuget")]
    [Secret]
    readonly string NuGetApiKey;

    [Solution(GenerateProjects = true)]
    readonly Solution Solution;

    [Required]
    [GitVersion(Framework = "net10.0", NoCache = true, NoFetch = true)]
    readonly GitVersion GitVersion;

    [Required]
    [GitRepository]
    readonly GitRepository GitRepository;

    AbsolutePath ArtifactsDirectory => RootDirectory / "Artifacts";

    AbsolutePath TestResultsDirectory => RootDirectory / "TestResults";

    string SemVer;

    Target Clean => _ => _
        .OnlyWhenDynamic(() => RunAllTargets || HasSourceChanges)
        .Executes(() =>
        {
            ArtifactsDirectory.CreateOrCleanDirectory();
            TestResultsDirectory.CreateOrCleanDirectory();
        });

    Target CalculateNugetVersion => _ => _
        .OnlyWhenDynamic(() => RunAllTargets || HasSourceChanges)
        .Executes(() =>
        {
            SemVer = GitVersion.SemVer;

            if (IsPullRequest)
            {
                Information(
                    "Branch spec {branchspec} is a pull request. Adding build number {buildnumber}",
                    BranchSpec, BuildNumber);

                SemVer = string.Join('.', GitVersion.SemVer.Split('.').Take(3).Union([BuildNumber]));
            }

            Information("SemVer = {semver}", SemVer);
        });

    bool IsPullRequest => GitHubActions?.IsPullRequest ?? false;

    Target Restore => _ => _
        .After(Clean)
        .OnlyWhenDynamic(() => RunAllTargets || HasSourceChanges)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution)
                .EnableNoCache()
                .SetConfigFile(RootDirectory / "nuget.config"));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .After(CalculateNugetVersion)
        .OnlyWhenDynamic(() => RunAllTargets || HasSourceChanges)
        .Executes(() =>
        {
            ReportSummary(s => s
                .WhenNotNull(SemVer, (summary, semVer) => summary
                    .AddPair("Version", semVer)));

            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .When(_ => GenerateBinLog is true, c => c
                    .SetBinaryLog(ArtifactsDirectory / $"{Solution.Core.AwesomeAssertions.Name}.binlog")
                )
                .EnableNoLogo()
                .EnableNoRestore()
                .SetVersion(SemVer)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion));
        });

    Target ApiChecks => _ => _
        .DependsOn(Compile)
        .OnlyWhenDynamic(() => RunAllTargets || HasSourceChanges)
        .Executes(() =>
        {
            Project project = Solution.Specs.Approval_Tests;

            DotNetTest(s => s
                .SetConfiguration(Configuration == Configuration.Debug ? "Debug" : "Release")
                .SetProcessEnvironmentVariable("DOTNET_CLI_UI_LANGUAGE", "en-US")
                .EnableNoBuild()
                .EnableListTests()
                .SetResultsDirectory(TestResultsDirectory)
                .CombineWith(cc => cc
                    .SetProjectFile(project)
                    .AddLoggers($"trx;LogFileName={project.Name}.trx")), completeOnFailure: true);
        });

    Project[] Projects =>
    [
        Solution.Specs.AwesomeAssertions_Specs,
        Solution.Specs.AwesomeAssertions_Equivalency_Specs,
        Solution.Specs.AwesomeAssertions_Extensibility_Specs,
        Solution.Specs.FSharp_Specs,
        Solution.Specs.VB_Specs
    ];

    /// <summary>
    /// Coverlet instruments by rewriting the assemblies in the output folder, and it cannot rewrite an assembly
    /// that the test host controller process has already loaded. The assembly level
    /// [AssertionEngineInitializer] attribute of AwesomeAssertions.Extensibility.Specs is defined in
    /// AwesomeAssertions, so enumerating the test assembly's attributes loads AwesomeAssertions.dll into that
    /// process before instrumentation starts. Coverlet then reports a successful test run without any coverage
    /// at all, so that project is excluded here deliberately rather than silently.
    /// </summary>
    bool CollectsCoverage(Project project) =>
        project.Name != Solution.Specs.AwesomeAssertions_Extensibility_Specs.Name;

    Target UnitTestsNetFramework => _ => _
        .Unlisted()
        .DependsOn(Compile)
        .OnlyWhenDynamic(() => EnvironmentInfo.IsWin && (RunAllTargets || HasSourceChanges))
        .Executes(() =>
        {
            DotNetTest(s => s
                    .SetConfiguration(Configuration.Debug)
                    .SetProcessEnvironmentVariable("DOTNET_CLI_UI_LANGUAGE", "en-US")
                    .EnableNoBuild()
                    .SetDataCollector("XPlat Code Coverage")
                    .SetResultsDirectory(TestResultsDirectory)
                    .AddRunSetting(
                        "DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.DoesNotReturnAttribute",
                        "DoesNotReturnAttribute")
                    .CombineWith(
                        Projects,
                        (settings, project) => settings
                            .SetProjectFile(project)
                            .SetFramework(NetFrameworkVersion)
                            .AddLoggers($"trx;LogFileName={project.Name}_{NetFrameworkVersion}.trx")),
                completeOnFailure: true);
        });

    Target UnitTestsCurrentDotNet => _ => _
        .Unlisted()
        .DependsOn(Compile)
        .OnlyWhenDynamic(() => RunAllTargets || HasSourceChanges)
        .Executes(() => RunUnitTests());

    void RunUnitTests()
    {
        var combinations = TestCombinations();
        var withCoverage = combinations.Where(v => CollectsCoverage(v.Project)).ToArray();
        var withoutCoverage = combinations.Except(withCoverage).ToArray();

        if (withCoverage.Length > 0)
        {
            DotNetTest(s => s
                    .SetConfiguration(Configuration.Debug)
                    .SetProcessEnvironmentVariable("DOTNET_CLI_UI_LANGUAGE", "en-US")
                    .EnableNoBuild()
                    .SetResultsDirectory(TestResultsDirectory)
                    .CombineWith(
                        withCoverage,
                        (settings, v) => settings
                            .SetProjectFile(v.Project)
                            .SetFramework(v.Framework)
                            .SetProcessAdditionalArguments(
                                "--coverlet",
                                // Passing this option without a value silently disables it again.
                                "--coverlet-does-not-return-attribute=DoesNotReturnAttribute",
                                // F# test hosts load FSharp.Core before instrumentation, so it cannot be rewritten.
                                "--coverlet-exclude=[FSharp.Core]*",
                                $"--coverlet-file-prefix={CoverageReportName(v.Project, v.Framework)}")),
                completeOnFailure: true);
        }

        if (withoutCoverage.Length > 0)
        {
            DotNetTest(s => s
                    .SetConfiguration(Configuration.Debug)
                    .SetProcessEnvironmentVariable("DOTNET_CLI_UI_LANGUAGE", "en-US")
                    .EnableNoBuild()
                    .SetResultsDirectory(TestResultsDirectory)
                    .CombineWith(
                        withoutCoverage,
                        (settings, v) => settings
                            .SetProjectFile(v.Project)
                            .SetFramework(v.Framework)),
                completeOnFailure: true);
        }

        VerifyCoverageWasCollected(withCoverage);
    }

    (Project Project, string Framework)[] TestCombinations() =>
    [
        ..from project in Projects
        from framework in project.GetTargetFrameworks()
        where framework != NetFrameworkVersion
        select (project, framework)
    ];

    static string CoverageReportName(Project project, string framework) => $"{project.Name}_{framework}";

    /// <summary>
    /// Coverlet reports a successful test run even when it collected nothing at all, either because it aborted
    /// instrumentation or because it skipped every module. Both cases have happened, so a missing or empty
    /// report has to be turned into a build failure explicitly.
    /// </summary>
    void VerifyCoverageWasCollected((Project Project, string Framework)[] combinations)
    {
        string[] withoutCoverage =
        [
            ..from combination in combinations
            let name = CoverageReportName(combination.Project, combination.Framework)
            where !TestResultsDirectory
                .GlobFiles($"{name}*.cobertura*.xml", $"**/{name}*.cobertura*.xml")
                .Any(ReportsAwesomeAssertions)
            select name
        ];

        if (withoutCoverage.Length > 0)
        {
            Assert.Fail($"No coverage of AwesomeAssertions was collected for: {string.Join(", ", withoutCoverage)}");
        }
    }

    static bool ReportsAwesomeAssertions(AbsolutePath report) =>
        System.IO.File.ReadAllText(report).Contains("<package name=\"AwesomeAssertions\"", StringComparison.Ordinal);

    Target UnitTests => _ => _
        .DependsOn(UnitTestsNetFramework)
        .DependsOn(UnitTestsCurrentDotNet);

    Target CodeCoverage => _ => _
        .DependsOn(TestFrameworks)
        .DependsOn(UnitTests)
        .OnlyWhenDynamic(() => RunAllTargets || HasSourceChanges)
        .Executes(() =>
        {
            ReportGenerator(s => s
                .SetProcessToolPath(NuGetToolPathResolver.GetPackageExecutable("ReportGenerator", "ReportGenerator.dll",
                    framework: "net8.0"))
                .SetTargetDirectory(TestResultsDirectory / "reports")
                // Coverlet writes "<prefix>.coverage.cobertura.<timestamp>.xml", the VSTest collector
                // "coverage.cobertura.xml" and Microsoft's MTP collector whatever we name it.
                .AddReports(TestResultsDirectory / "**/*cobertura*.xml")
                .AddReportTypes(
                    ReportTypes.lcov,
                    ReportTypes.HtmlInline_AzurePipelines_Dark)
                .AddFileFilters("-*.g.cs")
                .AddFileFilters("-*.nuget*")
                .SetAssemblyFilters("+AwesomeAssertions"));

            string link = TestResultsDirectory / "reports" / "index.html";
            Information($"Code coverage report: \x1b]8;;file://{link.Replace('\\', '/')}\x1b\\{link}\x1b]8;;\x1b\\");
        });

    Target VSTestFrameworks => _ => _
        .DependsOn(Compile)
        .OnlyWhenDynamic(() => RunAllTargets || HasSourceChanges)
        .Executes(() =>
        {
            var projects = Solution.TestFrameworks.VsTestPlatform.Projects;

            var testCombinations =
                from project in projects
                let frameworks = project.GetTargetFrameworks()
                let supportedFrameworks = EnvironmentInfo.IsWin ? frameworks : frameworks.Except([NetFrameworkVersion])
                from framework in supportedFrameworks
                select new { project, framework };

            DotNetTest(s => s
                    .SetConfiguration(Configuration.Debug)
                    .SetProcessEnvironmentVariable("DOTNET_CLI_UI_LANGUAGE", "en-US")
                    .SetProcessWorkingDirectory(RootDirectory / "Tests" / "TestFrameworks" / "VsTestPlatform")
                    .EnableNoBuild()
                    .SetDataCollector("XPlat Code Coverage")
                    .SetResultsDirectory(TestResultsDirectory)
                    .AddRunSetting(
                        "DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.DoesNotReturnAttribute",
                        "DoesNotReturnAttribute")
                    .CombineWith(
                        testCombinations,
                        (settings, v) => settings
                            .SetProjectFile(v.project)
                            .SetFramework(v.framework)
                            .AddLoggers($"trx;LogFileName={v.project.Name}_{v.framework}.trx")),
                completeOnFailure: true);
        });

    Target TestingPlatformFrameworks => _ => _
        .DependsOn(Compile)
        .OnlyWhenDynamic(() => RunAllTargets || HasSourceChanges)
        .Executes(() =>
        {
            var projects = Solution.TestFrameworks.Projects;

            var testCombinations =
                from project in projects
                let frameworks = project.GetTargetFrameworks()
                from framework in frameworks
                select new { project, framework };

            DotNetTest(s => s
                .SetConfiguration(Configuration.Debug)
                .SetProcessEnvironmentVariable("DOTNET_CLI_UI_LANGUAGE", "en-US")
                .EnableNoBuild()
                .CombineWith(
                    testCombinations,
                    (settings, v) => settings
                        .SetFramework(v.framework)
                        .SetProcessAdditionalArguments(
                            $"{v.project.Path}",
                            "--coverage" /*,
                            "--report-trx",
                            $"--report-trx-filename {v.project.Name}_{v.framework}.trx"*/,
                            $"--results-directory {TestResultsDirectory}")));
        });

    Target TestFrameworks => _ => _
        .DependsOn(VSTestFrameworks)
        .DependsOn(TestingPlatformFrameworks);

    Target Pack => _ => _
        .DependsOn(Clean)
        .DependsOn(CalculateNugetVersion)
        .DependsOn(ApiChecks)
        .DependsOn(TestFrameworks)
        .DependsOn(UnitTests)
        .DependsOn(CodeCoverage)
        .OnlyWhenDynamic(() => RunAllTargets || HasSourceChanges)
        .Executes(() =>
        {
            ReportSummary(s => s
                .WhenNotNull(SemVer, (c, semVer) => c
                    .AddPair("Packed version", semVer)));

            DotNetPack(s => s
                .SetProject(Solution.Core.AwesomeAssertions)
                .SetOutputDirectory(ArtifactsDirectory)
                .SetConfiguration(Configuration == Configuration.Debug ? "Debug" : "Release")
                .EnableNoLogo()
                .EnableNoRestore()
                .EnableContinuousIntegrationBuild() // Necessary for deterministic builds
                .SetVersion(SemVer));
        });

    Target Push => _ => _
        .DependsOn(Pack)
        .OnlyWhenDynamic(() => IsTag)
        .ProceedAfterFailure()
        .Executes(() =>
        {
            var packages = ArtifactsDirectory.GlobFiles("*.nupkg");

            Assert.NotEmpty(packages);

            DotNetNuGetPush(s => s
                .SetApiKey(NuGetApiKey)
                .EnableSkipDuplicate()
                .SetSource("https://api.nuget.org/v3/index.json")
                .EnableNoSymbols()
                .CombineWith(packages,
                    (v, path) => v.SetTargetPath(path)));
        });

    Target SpellCheck => _ => _
        .OnlyWhenDynamic(() => RunAllTargets || HasDocumentationChanges)
        .DependsOn(InstallNode)
        .ProceedAfterFailure()
        .Executes(() =>
        {
            NpmInstall(silent: true, workingDirectory: RootDirectory);
            NpmRun("cspell", silent: true);
        });

    Target InstallNode => _ => _
        .OnlyWhenDynamic(() => RunAllTargets || HasDocumentationChanges)
        .ProceedAfterFailure()
        .Executes(() =>
        {
            Initialize(RootDirectory);

            NpmFetchRuntime();

            ReportSummary(conf =>
            {
                if (HasCachedNodeModules)
                {
                    conf.AddPair("Skipped", "Downloading and extracting");
                }

                return conf;
            });
        });

    bool HasDocumentationChanges => Changes.Any(x => IsDocumentation(x));

    bool HasSourceChanges => Changes.Any(x => !IsDocumentation(x));

    static bool IsDocumentation(string x) =>
        x.StartsWith("docs") ||
        x.StartsWith("CONTRIBUTING.md") ||
        x.StartsWith("cSpell.json") ||
        x.StartsWith("LICENSE") ||
        x.StartsWith("package.json") ||
        x.StartsWith("package-lock.json") ||
        x.StartsWith("NodeVersion") ||
        x.StartsWith("README.md");

    string[] Changes =>
        Repository.Diff
            .Compare<TreeChanges>(TargetBranch, SourceBranch)
            .Where(x => x.Exists)
            .Select(x => x.Path)
            .ToArray();

    Repository Repository => new(GitRepository.LocalDirectory);

    Tree TargetBranch => Repository.Branches[PullRequestBase].Tip.Tree;

    Tree SourceBranch => Repository.Branches[Repository.Head.FriendlyName].Tip.Tree;

    bool RunAllTargets => string.IsNullOrWhiteSpace(PullRequestBase) || Changes.Any(x => x.StartsWith("Build"));

    bool IsTag => BranchSpec != null && BranchSpec.Contains("refs/tags", StringComparison.OrdinalIgnoreCase);
}
