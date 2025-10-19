namespace AwesomeAssertions.Execution;

internal class MSTestFramework(string assemblyName) : LateBoundTestFramework
{
    protected override string ExceptionFullName => "Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException";

    protected internal override string AssemblyName => assemblyName;
}
