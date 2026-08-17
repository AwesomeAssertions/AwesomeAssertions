using System;
using System.Runtime.CompilerServices;
using AwesomeAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NativeAot.IntegrationTests;

[TestClass]
public sealed class MonitorNativeAotTests
{
    [TestMethod]
    public void Monitor_throws_when_dynamic_code_is_not_supported()
    {
        // This project is also executed via regular dotnet test, where dynamic code is available.
        // We only assert the failure path when running the published NativeAOT test host.
        if (RuntimeFeature.IsDynamicCodeSupported)
        {
            return;
        }

        var subject = new NotifySource();

        var exception = Assert.Throws<Exception>(() =>
        {
            using var _ = subject.Monitor();
        });

        Assert.AreEqual("Dynamic code generation is not supported on this platform.", exception.Message);
    }

    private sealed class NotifySource
    {
#pragma warning disable CS0067 // Event is never used
        // ReSharper disable once EventNeverSubscribedTo.Local
        public event EventHandler SomethingHappened;
#pragma warning restore CS0067 // Event is never used
    }
}
