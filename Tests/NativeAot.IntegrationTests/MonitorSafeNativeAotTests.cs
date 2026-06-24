using System;
using System.ComponentModel;
using AwesomeAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NativeAot.IntegrationTests;

[TestClass]
public sealed class MonitorSafeNativeAotTests
{
    [TestMethod]
    public void MonitorSafe_captures_PropertyChanged_event()
    {
        var subject = new NotifySource();

        using var monitor = subject.MonitorSafe(options => options
            .UsingSafeEventHandlerAdapter<PropertyChangedEventHandler>(record =>
                (sender, args) => record([sender, args])));

        subject.Raise(nameof(NotifySource.Name));

        monitor
            .Should().Raise(nameof(NotifySource.PropertyChanged))
            .WithSender(subject)
            .WithArgs<PropertyChangedEventArgs>(args => args.PropertyName == nameof(NotifySource.Name));
    }

    [TestMethod]
    public void MonitorSafe_rejects_two_parameter_custom_delegate_without_adapter()
    {
        var subject = new CustomDelegateSource();

        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            using var _ = subject.MonitorSafe();
        });

        StringAssert.Contains(exception.Message, "UsingSafeEventHandlerAdapter");
    }

    [TestMethod]
    public void MonitorSafe_rejects_unsupported_delegate_shape()
    {
        var subject = new UnsupportedDelegateSource();

        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            using var _ = subject.MonitorSafe();
        });

        StringAssert.Contains(exception.Message, "safe mode");
    }

    [TestMethod]
    public void MonitorSafe_allows_unsupported_delegate_shape_with_per_call_adapter()
    {
        var subject = new UnsupportedDelegateSource();

        using var monitor = subject.MonitorSafe(options => options
            .UsingSafeEventHandlerAdapter<UnsupportedHandler>(record =>
                (first, second, third) => record([first, second, third])));

        subject.Raise("left", 7, "right");

        monitor
            .Should().Raise(nameof(UnsupportedDelegateSource.CustomEvent))
            .WithArgs<int>(value => value == 7)
            .WithArgs<string>(value => value == "right");
    }

    private sealed class NotifySource : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Raise(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private delegate void StringPayloadHandler(object sender, string payload);

    private sealed class CustomDelegateSource
    {
        // ReSharper disable EventNeverSubscribedTo.Local
        public event StringPayloadHandler CustomEvent;

        // ReSharper disable once UnusedMember.Local
        public void Raise(string payload) => CustomEvent?.Invoke(this, payload);
    }

    private delegate void UnsupportedHandler(string first, int second, string third);

    private sealed class UnsupportedDelegateSource
    {
#pragma warning disable CS0067 // Event is never used
        public event UnsupportedHandler CustomEvent;
#pragma warning restore CS0067 // Event is never used

        public void Raise(string first, int second, string third) => CustomEvent?.Invoke(first, second, third);
    }
}
