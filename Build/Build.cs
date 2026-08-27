using System;
using System.Collections.Generic;
using System.Linq;
using Fallout.Common;
using Fallout.Common.CI.GitHubActions;
using Fallout.Common.Execution;
using Fallout.Common.Git;
using Fallout.Common.IO;
using Fallout.Common.Tooling;
using Fallout.Common.Tools.DotNet;
using Fallout.Common.Tools.GitVersion;
using Fallout.Common.Tools.ReportGenerator;
using Fallout.Common.Utilities;
using Fallout.Common.Utilities.Collections;
using Fallout.Solutions;
using LibGit2Sharp;
using static CustomNpmTasks;
using static Fallout.Common.Tools.DotNet.DotNetTasks;
using static Fallout.Common.Tools.ReportGenerator.ReportGeneratorTasks;
using static Serilog.Log;

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
                .CombineWith(x => x.SetProjectFile(project)),
                completeOnFailure: true);
        });

    Project[] TestProjects =>
    [
        Solution.Specs.AwesomeAssertions_Specs,
        Solution.Specs.AwesomeAssertions_Equivalency_Specs,
        Solution.Specs.AwesomeAssertions_Extensibility_Specs,
        Solution.Specs.FSharp_Specs,
        Solution.Specs.VB_Specs
    ];

    /// <summary>
    /// We need to provide test settings.
    /// By default, code with [DebuggerNonUserCode] is excluded.
    /// But this is used several times in AA code.
    /// We can use the "runsettings" format (VSTest) also for the MTP platform tests.
    /// </summary>
    static AbsolutePath CoverageSettingsFile => RootDirectory / "Tests" / "CodeCoverage.runsettings";

    Target UnitTestsNetFramework => _ => _
        .Unlisted()
        .DependsOn(Compile)
        .OnlyWhenDynamic(() => EnvironmentInfo.IsWin && (RunAllTargets || HasSourceChanges))
        .Executes(() => RunUnitTests(TestProjects, _ => [NetFrameworkVersion]));

    Target UnitTestsCurrentDotNet => _ => _
        .Unlisted()
        .DependsOn(Compile)
        .OnlyWhenDynamic(() => RunAllTargets || HasSourceChanges)
        .Executes(() => RunUnitTests(TestProjects, p => p.GetTargetFrameworks().Except([NetFrameworkVersion])));

    void RunUnitTests(IEnumerable<Project> testProjects, Func<Project, IEnumerable<string>> frameworksSelector)
    {
        var coverageFiles = new List<string>();

        DotNetTest(s => s
                .SetConfiguration(Configuration.Debug)
                .SetProcessEnvironmentVariable("DOTNET_CLI_UI_LANGUAGE", "en-US")
                .EnableNoBuild()
                .SetResultsDirectory(TestResultsDirectory)
                .CombineWith(
                    testProjects.Where(p => p.GetTargetFrameworks().Intersect(frameworksSelector(p)).Any()),
                    (settings, project) => settings
                        .SetProjectFile(project)
                        .CombineWith(
                            frameworksSelector(project),
                            (frameworkSettings, framework) =>
                            {
                                var coverageFile = $"{project.Name}_{framework}.cobertura.xml";
                                coverageFiles.Add(coverageFile);

                                return frameworkSettings
                                    .SetFramework(framework)
                                    .SetProcessAdditionalArguments(
                                        "--coverage",
                                        "--coverage-output-format=cobertura",
                                        $"--coverage-output={coverageFile}",
                                        "--coverage-settings",
                                        CoverageSettingsFile);
                            })),
            completeOnFailure: true);

        var missingFiles = coverageFiles.Where(x => !(TestResultsDirectory / x).FileExists()).ToArray();
        missingFiles.ForEach(x => Assert.Fail($"Missing coverage file: {x}"));
        var missingCoverage = coverageFiles.Where(x => !ReportsAwesomeAssertions(TestResultsDirectory / x)).ToArray();
        missingCoverage.ForEach(x => Assert.Fail($"Missing coverage of AwesomeAssertions in: {x}"));
    }

    static bool ReportsAwesomeAssertions(AbsolutePath report) =>
        System.IO.File.ReadAllText(report).Contains("name=\"AwesomeAssertions\"", StringComparison.Ordinal);

    Target UnitTests => _ => _
        .DependsOn(UnitTestsNetFramework)
        .DependsOn(UnitTestsCurrentDotNet);

    Target CodeCoverage => _ => _
        .DependsOn(TestFrameworks)
        .DependsOn(UnitTests)
        .OnlyWhenDynamic(() => RunAllTargets || HasSourceChanges)
        .Executes(() =>
        {
            string generator = NuGetToolPathResolver.GetPackageExecutable(
                "ReportGenerator", "ReportGenerator.dll", framework: "net10.0");
            ReportGenerator(s => s
                .SetProcessToolPath(generator)
                .SetTargetDirectory(TestResultsDirectory / "reports")
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
                    .SetDataCollector("Code Coverage")
                    .SetSettingsFile(CoverageSettingsFile)
                    .SetResultsDirectory(TestResultsDirectory)
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
        .Executes(() => RunUnitTests(Solution.TestFrameworks.Projects, p => p.GetTargetFrameworks()));

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
