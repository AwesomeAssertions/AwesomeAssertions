using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
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

    Target Clean => d => d
        .OnlyWhenDynamic(() => RunAllTargets || HasSourceChanges)
        .Executes(() =>
        {
            ArtifactsDirectory.CreateOrCleanDirectory();
            TestResultsDirectory.CreateOrCleanDirectory();
        });

    Target CalculateNugetVersion => d => d
        .OnlyWhenDynamic(() => RunAllTargets || HasSourceChanges)
        .Executes(() =>
        {
            SemVer = GitVersion.SemVer;

            if (IsPullRequest)
            {
                Information(
                    "Branch spec {BranchSpec} is a pull request. Adding build number {BuildNumber}",
                    BranchSpec, BuildNumber);

                SemVer = string.Join('.', GitVersion.SemVer.Split('.').Take(3).Union([BuildNumber]));
            }

            Information("SemVer = {SemVer}", SemVer);
        });

    bool IsPullRequest => GitHubActions?.IsPullRequest ?? false;

    Target Restore => d => d
        .After(Clean)
        .OnlyWhenDynamic(() => RunAllTargets || HasSourceChanges)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution)
                .EnableNoCache()
                .SetConfigFile(RootDirectory / "nuget.config"));
        });

    Target Compile => d => d
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

    Target ApiChecks => d => d
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

    Target UnitTestsNetFramework => d => d
        .Unlisted()
        .DependsOn(Compile)
        .OnlyWhenDynamic(() => EnvironmentInfo.IsWin && (RunAllTargets || HasSourceChanges))
        .Executes(() => RunUnitTests(TestProjects, _ => [NetFrameworkVersion]));

    Target UnitTestsCurrentDotNet => d => d
        .Unlisted()
        .DependsOn(Compile)
        .OnlyWhenDynamic(() => RunAllTargets || HasSourceChanges)
        .Executes(() => RunUnitTests(TestProjects, p => p.GetTargetFrameworks().Except([NetFrameworkVersion])));

    Target UnitTests => d => d
        .DependsOn(UnitTestsNetFramework)
        .DependsOn(UnitTestsCurrentDotNet);

    Target TestingPlatformFrameworks => d => d
        .DependsOn(Compile)
        .OnlyWhenDynamic(() => RunAllTargets || HasSourceChanges)
        .Executes(() => RunUnitTests(Solution.TestFrameworks.Projects, p => p.GetTargetFrameworks()));

    Target TestFrameworks => d => d
        .DependsOn(VSTestFrameworks)
        .DependsOn(TestingPlatformFrameworks);

    Target CodeCoverage => d => d
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

    Target VSTestFrameworks => d => d
        .DependsOn(Compile)
        .OnlyWhenDynamic(() => RunAllTargets || HasSourceChanges)
        .Executes(() =>
        {
            var testCombinations =
                from project in Solution.TestFrameworks.VsTestPlatform.Projects
                let frameworks = project.GetTargetFrameworks()
                let supportedFrameworks = EnvironmentInfo.IsWin ? frameworks : frameworks.Except([NetFrameworkVersion])
                from framework in supportedFrameworks
                select new { project, framework };

            var coverageLogFiles = new List<AbsolutePath>();
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
                        (settings, v) =>
                        {
                            string coverageLogFile = $"{v.project.Name}_{v.framework}.trx";
                            coverageLogFiles.Add(TestResultsDirectory / coverageLogFile);

                            return settings
                                .SetProjectFile(v.project)
                                .SetFramework(v.framework)
                                .AddLoggers($"trx;LogFileName={coverageLogFile}");
                        }),
                completeOnFailure: true);

            // Remove duplicated results (we remove the GUID named)
            TestResultsDirectory.GlobDirectories("*").Where(x => Guid.TryParse(x.Name, out Guid _)).ForEach(x =>
            {
                Information("Deleting test results directory: {Directory}", x);
                x.DeleteDirectory();
            });

            // Validate test result files
            AbsolutePath[] missingFiles = coverageLogFiles.Where(x => !x.FileExists()).ToArray();
            missingFiles.ForEach(x => Assert.Fail($"Missing coverage log file: {x}"));
            AbsolutePath[] coverageFiles = coverageLogFiles.SelectMany(ExtractCoverageFiles).ToArray();
            Assert.Count(coverageFiles, testCombinations.Count());
            missingFiles = coverageFiles.Where(x => !x.FileExists()).ToArray();
            missingFiles.ForEach(x => Assert.Fail($"Missing coverage file: {x}"));
            AbsolutePath[] missingCoverage = coverageFiles.Where(x => !ReportsAwesomeAssertions(x)).ToArray();
            missingCoverage.ForEach(x => Assert.Fail($"Missing coverage of AwesomeAssertions in: {x}"));
        });

    void RunUnitTests(IEnumerable<Project> testProjects, Func<Project, IEnumerable<string>> frameworksSelector)
    {
        var coverageFiles = new List<string>();

        DotNetTest(s => s
                .SetConfiguration(Configuration.Debug)
                .SetProcessEnvironmentVariable("DOTNET_CLI_UI_LANGUAGE", "en-US")
                // We do not need TUnits artifacts.
                .SetProcessEnvironmentVariable("TUNIT_DISABLE_HTML_REPORTER", "true")
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
                                        $"--coverage-output={coverageFile}",
                                        "--coverage-settings",
                                        CoverageSettingsFile);
                            })),
            completeOnFailure: true);

        string[] missingFiles = coverageFiles.Where(x => !(TestResultsDirectory / x).FileExists()).ToArray();
        missingFiles.ForEach(x => Assert.Fail($"Missing coverage file: {x}"));
        string[] missingCoverage = coverageFiles.Where(x => !ReportsAwesomeAssertions(TestResultsDirectory / x)).ToArray();
        missingCoverage.ForEach(x => Assert.Fail($"Missing coverage of AwesomeAssertions in: {x}"));
    }

    static IEnumerable<AbsolutePath> ExtractCoverageFiles(AbsolutePath coverageLogFile)
    {
        XmlDocument document = new();
        document.Load(coverageLogFile);
        XmlNamespaceManager nsManager = new(document.NameTable);
        nsManager.AddNamespace("vs", "http://microsoft.com/schemas/VisualStudio/TeamTest/2010");

        const string xpath =
            "//vs:CollectorDataEntries/vs:Collector[@uri='datacollector://microsoft/CodeCoverage/2.0']"
            + "/vs:UriAttachments/vs:UriAttachment/vs:A/@href";

        XmlNodeList nodes = document.SelectNodes(xpath, nsManager);
        return nodes?.OfType<XmlNode>().SelectMany(x =>
        {
            int pos = x.Value?.IndexOf("\\") ?? -1;
            string fileName = pos >= 0 ? x.Value?[(pos + 1)..] ?? string.Empty : x.Value;
            return coverageLogFile.Parent.GlobFiles("**/" + fileName);
        }) ?? [];
    }

    static bool ReportsAwesomeAssertions(AbsolutePath coverageFile)
    {
        bool coverageExists = System.IO.File.ReadAllText(coverageFile)
            .Contains("name=\"AwesomeAssertions\"", StringComparison.Ordinal);
        Information("Coverage in {File} exists: {Exists}", coverageFile, coverageExists);
        return coverageExists;
    }

    Target Pack => d => d
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

    Target Push => d => d
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

    Target SpellCheck => d => d
        .OnlyWhenDynamic(() => RunAllTargets || HasDocumentationChanges)
        .DependsOn(InstallNode)
        .ProceedAfterFailure()
        .Executes(() =>
        {
            NpmInstall(silent: true, workingDirectory: RootDirectory);
            NpmRun("cspell", silent: true);
        });

    Target InstallNode => d => d
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
