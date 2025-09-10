using System;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions;

// suppress having multiple classes with different names in one file
#pragma warning disable AV1507,MA0048

internal static class Foo
{
    public static bool AssertRaisesS3655(AssertionChain assertionChain, object subject)
    {
        assertionChain
            .ForCondition(subject is not null);

        if (assertionChain.Succeeded)
        {
            assertionChain
                .ForCondition(Check(subject.GetType())); // Raises S3655
        }

        return assertionChain.Succeeded;
    }

    public static bool AssertRaisesS3655<T>(AssertionChain assertionChain, T? subject)
        where T : struct, Enum
    {
        assertionChain
            .ForCondition(subject is not null);

        if (assertionChain.Succeeded)
        {
            assertionChain
                .ForCondition(Check(subject.GetType())); // Raises S3655
        }

        return assertionChain.Succeeded;
    }

    public static bool AssertRaisesS3655(AssertionChain assertionChain, double? subject)
    {
        assertionChain
            .ForCondition(subject is not null);

        if (assertionChain.Succeeded)
        {
            assertionChain
                .ForCondition(Check(subject.GetType())); // Raises S3655
        }

        return assertionChain.Succeeded;
    }

    public static bool AssertWithExclamationMark(AssertionChain assertionChain, object subject)
    {
        assertionChain
            .ForCondition(subject is not null);

        if (assertionChain.Succeeded)
        {
            assertionChain
                .ForCondition(Check(subject!.GetType()));
        }

        return assertionChain.Succeeded;
    }

    public static bool AssertWithExclamationMark<T>(AssertionChain assertionChain, T? subject)
        where T : struct, Enum
    {
        assertionChain
            .ForCondition(subject is not null);

        if (assertionChain.Succeeded)
        {
            assertionChain
                .ForCondition(Check(subject!.Value));
        }

        return assertionChain.Succeeded;
    }

    public static bool AssertWithExclamationMark(AssertionChain assertionChain, double? subject)
    {
        assertionChain
            .ForCondition(subject is not null);

        if (assertionChain.Succeeded)
        {
            assertionChain
                .ForCondition(Check(subject!.Value));
        }

        return assertionChain.Succeeded;
    }

    private static bool Check<T>(T value)
    {
        _ = value;
        return true;
    }
}

internal abstract class FooBase<T>
{
    public T Subject { get; }
}

internal sealed class FooObject : FooBase<object>
{
    public bool AssertRaisesS3655(AssertionChain assertionChain)
    {
        assertionChain
            .ForCondition(Subject is not null);

        if (assertionChain.Succeeded)
        {
            assertionChain
                .ForCondition(Check(Subject.GetType())); // Raises S3655
        }

        return assertionChain.Succeeded;
    }

    public bool AssertWithExclamationMark(AssertionChain assertionChain)
    {
        assertionChain
            .ForCondition(Subject is not null);

        if (assertionChain.Succeeded)
        {
            assertionChain
                .ForCondition(Check(Subject!.GetType()));
        }

        return assertionChain.Succeeded;
    }

    private static bool Check<T>(T value)
    {
        _ = value;
        return true;
    }
}

internal sealed class FooEnum<T> : FooBase<T?>
    where T : struct, Enum
{
    public bool AssertRaisesS3655(AssertionChain assertionChain)
    {
        assertionChain
            .ForCondition(Subject is not null);

        if (assertionChain.Succeeded)
        {
            assertionChain
                .ForCondition(Check(Subject.Value)); // Raises S3655
        }

        return assertionChain.Succeeded;
    }

    public bool AssertWithExclamationMark(AssertionChain assertionChain)
    {
        assertionChain
            .ForCondition(Subject is not null);

        if (assertionChain.Succeeded)
        {
            assertionChain
                .ForCondition(Check(Subject!.Value));
        }

        return assertionChain.Succeeded;
    }

    private static bool Check(T value)
    {
        _ = value;
        return true;
    }
}

internal sealed class FooDouble : FooBase<double?>
{
    public bool AssertRaisesS3655(AssertionChain assertionChain)
    {
        assertionChain
            .ForCondition(Subject is not null);

        if (assertionChain.Succeeded)
        {
            assertionChain
                .ForCondition(Check(Subject.Value)); // Raises S3655
        }

        return assertionChain.Succeeded;
    }

    public bool AssertWithExclamationMark(AssertionChain assertionChain)
    {
        assertionChain
            .ForCondition(Subject is not null);

        if (assertionChain.Succeeded)
        {
            assertionChain
                .ForCondition(Check(Subject!.Value));
        }

        return assertionChain.Succeeded;
    }

    private static bool Check(double value)
    {
        _ = value;
        return true;
    }
}
