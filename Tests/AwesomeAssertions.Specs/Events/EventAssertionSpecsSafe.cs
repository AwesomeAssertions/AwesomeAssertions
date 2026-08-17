#if NET6_0_OR_GREATER

using System;
using System.ComponentModel;
using Xunit;

namespace AwesomeAssertions.Specs.Events;

public class EventAssertionSpecsSafe
{
    [Fact]
    public void When_monitoring_event_handler_events_with_safe_monitor_it_should_capture_events()
    {
        // Arrange
        var subject = new SafeEventSource();

        using var monitor = subject.MonitorSafe();

        // Act
        subject.RaiseSimple();

        // Assert
        monitor.Should().Raise(nameof(SafeEventSource.Simple));
    }

    [Fact]
    public void When_monitoring_generic_event_handler_events_with_safe_monitor_it_should_capture_events()
    {
        // Arrange
        var subject = new SafeEventSource();

        using var monitor = subject.MonitorSafe();

        // Act
        subject.RaiseGeneric();

        // Assert
        monitor.Should().Raise(nameof(SafeEventSource.Generic));
    }

    [Fact]
    public void When_monitoring_property_changed_event_with_safe_monitor_it_should_capture_events()
    {
        // Arrange
        var subject = new SafePropertyChangedSource();

        using var monitor = subject.MonitorSafe(options => options
            .UsingSafeEventHandlerAdapter<PropertyChangedEventHandler>(record =>
                (sender, args) => record([sender, args])));

        // Act
        subject.Raise(nameof(SafePropertyChangedSource.SomeProperty));

        // Assert
        monitor
            .Should().Raise(nameof(SafePropertyChangedSource.PropertyChanged))
            .WithSender(subject)
            .WithArgs<PropertyChangedEventArgs>(args => args.PropertyName == nameof(SafePropertyChangedSource.SomeProperty));
    }

    [Fact]
    public void When_monitoring_two_parameter_custom_delegate_event_with_safe_monitor_without_adapter_it_should_throw()
    {
        // Arrange
        var subject = new TwoParameterCustomDelegateSource();

        // Act
        Action act = () =>
        {
            using var _ = subject.MonitorSafe();
        };

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Unable to monitor event 'CustomEvent' in safe mode. *UsingSafeEventHandlerAdapter*");
    }

    [Fact]
    public void When_monitoring_non_event_handler_style_delegate_with_safe_monitor_it_should_throw()
    {
        // Arrange
        var subject = new UnsupportedCustomDelegateSource();

        // Act
        Action act = () =>
        {
            using var _ = subject.MonitorSafe();
        };

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Unable to monitor event 'CustomEvent' in safe mode. *UsingSafeEventHandlerAdapter*");
    }

    [Fact]
    public void When_monitoring_non_event_handler_style_delegate_with_per_call_safe_adapter_it_should_capture_events()
    {
        // Arrange
        var subject = new UnsupportedCustomDelegateSource();

        using var monitor = subject.MonitorSafe(options => options
            .UsingSafeEventHandlerAdapter<ThreeParameterDelegate>(record =>
                (first, second, third) => record([first, second, third])));

        // Act
        subject.Raise("hello", 42, "world");

        // Assert
        monitor
            .Should().Raise(nameof(UnsupportedCustomDelegateSource.CustomEvent))
            .WithArgs<int>(value => value == 42)
            .WithArgs<string>(value => value == "world");
    }

    private sealed class SafeEventSource
    {
        public event EventHandler Simple;

        public event EventHandler<EventArgs> Generic;

        public void RaiseSimple() => Simple?.Invoke(this, EventArgs.Empty);

        public void RaiseGeneric() => Generic?.Invoke(this, EventArgs.Empty);
    }

    private sealed class SafePropertyChangedSource : INotifyPropertyChanged
    {
        public string SomeProperty { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Raise(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private delegate void StringPayloadEventHandler(object sender, string payload);

    private sealed class TwoParameterCustomDelegateSource
    {
#pragma warning disable MA0046, RCS1159
        public event StringPayloadEventHandler CustomEvent;
#pragma warning restore MA0046, RCS1159

        public void Raise(string payload) => CustomEvent?.Invoke(this, payload);
    }

    private delegate void ThreeParameterDelegate(string first, int second, string third);

    private sealed class UnsupportedCustomDelegateSource
    {
#pragma warning disable MA0046
        public event ThreeParameterDelegate CustomEvent;
#pragma warning restore MA0046

        public void Raise(string first, int second, string third) => CustomEvent?.Invoke(first, second, third);
    }
}

#endif
