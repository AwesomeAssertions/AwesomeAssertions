#if !NETSTANDARD2_0

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using AwesomeAssertions.Common;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Events;

/// <summary>
/// Tracks the events an object raises.
/// </summary>
internal sealed class EventMonitor<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents)] T> : IMonitor<T>
{
    private readonly WeakReference subject;

    private readonly ConcurrentDictionary<string, EventRecorder> recorderMap = new();

    private EventMonitor(object eventSource, EventMonitorOptions options)
    {
        Guard.ThrowIfArgumentIsNull(eventSource, nameof(eventSource), "Cannot monitor the events of a <null> object.");
        Guard.ThrowIfArgumentIsNull(options, nameof(options), "Event monitor needs configuration.");

        this.options = options;
        subject = new WeakReference(eventSource);
    }

    [RequiresDynamicCode("Event monitoring requires runtime-generated delegates.")]
    public static EventMonitor<T> Create(object eventSource, EventMonitorOptions options)
    {
        var monitor = new EventMonitor<T>(eventSource, options);
        monitor.Attach(typeof(T), monitor.options.TimestampProvider);
        return monitor;
    }

    public static EventMonitor<T> CreateSafe(object eventSource, EventMonitorOptions options)
    {
        var monitor = new EventMonitor<T>(eventSource, options);
        monitor.AttachSafe(typeof(T), monitor.options.TimestampProvider);
        return monitor;
    }

    public T Subject => (T)subject.Target;

    private readonly ThreadSafeSequenceGenerator threadSafeSequenceGenerator = new();
    private readonly EventMonitorOptions options;

    public EventMetadata[] MonitoredEvents =>
        recorderMap
            .Values
            .Select(recorder => new EventMetadata(recorder.EventName, recorder.EventHandlerType))
            .ToArray();

    public OccurredEvent[] OccurredEvents
    {
        get
        {
            IEnumerable<OccurredEvent> query =
                from eventName in recorderMap.Keys
                let recording = GetRecordingFor(eventName)
                from @event in recording
                orderby @event.Sequence
                select @event;

            return query.ToArray();
        }
    }

    public void Clear()
    {
        foreach (EventRecorder recorder in recorderMap.Values)
        {
            recorder.Reset();
        }
    }

    public EventAssertions<T> Should()
    {
        return new EventAssertions<T>(this, AssertionChain.GetOrCreate());
    }

    public IEventRecording GetRecordingFor(string eventName)
    {
        if (!recorderMap.TryGetValue(eventName, out EventRecorder recorder))
        {
            throw new InvalidOperationException($"Not monitoring any events named \"{eventName}\".");
        }

        return recorder;
    }

    [RequiresDynamicCode("Event monitoring requires runtime-generated delegates.")]
    private void Attach(Type typeDefiningEventsToMonitor, Func<DateTime> utcNow)
    {
        if (subject.Target is null)
        {
            throw new InvalidOperationException("Cannot monitor events on garbage-collected object");
        }

        EventInfo[] events = GetPublicEvents(typeDefiningEventsToMonitor);

        if (events.Length == 0)
        {
            throw new InvalidOperationException($"Type {typeDefiningEventsToMonitor.Name} does not expose any events.");
        }

        foreach (EventInfo eventInfo in events)
        {
            AttachEventHandler(eventInfo, utcNow);
        }
    }

    private void AttachSafe(Type typeDefiningEventsToMonitor, Func<DateTime> utcNow)
    {
        if (subject.Target is null)
        {
            throw new InvalidOperationException("Cannot monitor events on garbage-collected object");
        }

        EventInfo[] events = GetPublicEvents(typeDefiningEventsToMonitor);

        if (events.Length == 0)
        {
            throw new InvalidOperationException($"Type {typeDefiningEventsToMonitor.Name} does not expose any events.");
        }

        foreach (EventInfo eventInfo in events)
        {
            AttachEventHandlerSafe(eventInfo, utcNow);
        }
    }

    [UnconditionalSuppressMessage("Trimming", "IL2070",
        Justification = "Event reflection is a best-effort feature and not part of core safe mode support.")]
    private static EventInfo[] GetPublicEvents(Type type)
    {
        if (!type.IsInterface)
        {
            return type.GetEvents();
        }

        return new[] { type }
            .Concat(type.GetInterfaces())
            .SelectMany(i => i.GetEvents())
            .ToArray();
    }

    public void Dispose()
    {
        foreach (EventRecorder recorder in recorderMap.Values)
        {
            DisposeSafeIfRequested(recorder);
        }

        recorderMap.Clear();
    }

    private void DisposeSafeIfRequested(IDisposable recorder)
    {
        try
        {
            recorder.Dispose();
        }
        catch when (options.ShouldIgnoreEventAccessorExceptions)
        {
            // ignore
        }
    }

    [RequiresDynamicCode("Event monitoring requires runtime-generated delegates.")]
    private void AttachEventHandler(EventInfo eventInfo, Func<DateTime> utcNow)
    {
        if (!recorderMap.TryGetValue(eventInfo.Name, out _))
        {
            var recorder = new EventRecorder(subject.Target, eventInfo.Name, utcNow, threadSafeSequenceGenerator);

            if (recorderMap.TryAdd(eventInfo.Name, recorder))
            {
                AttachEventHandler(eventInfo, recorder);
            }
        }
    }

    private void AttachEventHandlerSafe(EventInfo eventInfo, Func<DateTime> utcNow)
    {
        if (!recorderMap.TryGetValue(eventInfo.Name, out _))
        {
            var recorder = new EventRecorder(subject.Target, eventInfo.Name, utcNow, threadSafeSequenceGenerator);

            if (recorderMap.TryAdd(eventInfo.Name, recorder))
            {
                AttachEventHandlerSafe(eventInfo, recorder);
            }
        }
    }

    [RequiresDynamicCode("Event monitoring requires runtime-generated delegates.")]
    private void AttachEventHandler(EventInfo eventInfo, EventRecorder recorder)
    {
        try
        {
            recorder.Attach(subject, eventInfo);
        }
        catch when (options.ShouldIgnoreEventAccessorExceptions)
        {
            if (!options.ShouldRecordEventsWithBrokenAccessor)
            {
                recorderMap.TryRemove(eventInfo.Name, out _);
            }
        }
    }

    private void AttachEventHandlerSafe(EventInfo eventInfo, EventRecorder recorder)
    {
        try
        {
            if (!recorder.TryAttachSafe(subject, eventInfo, options, out string reason))
            {
                recorderMap.TryRemove(eventInfo.Name, out _);

                throw new InvalidOperationException(
                    $"Unable to monitor event '{eventInfo.Name}' in safe mode. {reason}");
            }
        }
        catch when (options.ShouldIgnoreEventAccessorExceptions)
        {
            if (!options.ShouldRecordEventsWithBrokenAccessor)
            {
                recorderMap.TryRemove(eventInfo.Name, out _);
            }
        }
    }
}

#endif
