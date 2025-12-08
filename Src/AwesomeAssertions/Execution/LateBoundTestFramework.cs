using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
#if NET6_0_OR_GREATER
using System.Runtime.Loader;
#endif

namespace AwesomeAssertions.Execution;

#nullable enable

internal abstract class LateBoundTestFramework : ITestFramework
{
    private readonly bool loadAssembly;
    private Func<string, Exception> exceptionFactory;

    protected LateBoundTestFramework(bool loadAssembly = false)
    {
        this.loadAssembly = loadAssembly;
        exceptionFactory = _ => throw new InvalidOperationException($"{nameof(IsAvailable)} must be called first.");
    }

    [DoesNotReturn]
    public void Throw(string message) => throw exceptionFactory(message);

    public bool IsAvailable
    {
        get
        {
            Assembly? assembly = FindExceptionAssembly();
            Type? exceptionType = assembly?.GetType(ExceptionFullName);

            exceptionFactory = exceptionType is not null
                ? message => (Exception)Activator.CreateInstance(exceptionType, message)!
                : _ => throw new InvalidOperationException($"{GetType().Name} is not available");

            return exceptionType is not null;
        }
    }

    protected internal abstract string AssemblyName { get; }

    protected abstract string ExceptionFullName { get; }

    private static IEnumerable<Assembly> GetAssemblies()
    {
#if NET6_0_OR_GREATER
        // In some constellations a test framework assembly might have been loaded more than once. Make sure we get the correct one:
        // AppDomain.GetAssemblies: Gets the assemblies that have been loaded into 
        // the execution context of this application domain.
        // And in the case of NUnit4.Mtp.Specs this returns nunit.framework twice, with the first one 
        // being the wrong assembly, which is a different one than used for running the tests.
        //
        // So we are looking for the nunit.framework assembly which is used during test execution,
        // which we get through AssemblyLoadContext.
        AssemblyLoadContext loadContext = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly()) ??
            AssemblyLoadContext.Default;

        return loadContext.Assemblies;
#else
        return AppDomain.CurrentDomain.GetAssemblies();
#endif
    }

    private Assembly? FindExceptionAssembly()
    {
        Assembly? assembly = GetAssemblies().FirstOrDefault(a => a.GetName().Name == AssemblyName);

        if (assembly is null && loadAssembly)
        {
            try
            {
                return Assembly.Load(new AssemblyName(AssemblyName));
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (FileLoadException)
            {
                return null;
            }
        }

        return assembly;
    }
}
