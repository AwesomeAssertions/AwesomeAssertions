using System;
using System.Globalization;
using AwesomeAssertions.Common;
using static System.FormattableString;

namespace AwesomeAssertions.Equivalency.Steps;

/// <summary>
/// Attempts to convert the subject's property value to the expected type.
/// </summary>
/// <remarks>
/// Whether or not the conversion is attempted depends on the <see cref="ConversionSelector"/>.
/// </remarks>
public class AutoConversionStep : IEquivalencyStep
{
    public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context,
        IValidateChildNodeEquivalency valueChildNodes)
    {
        if (!context.Options.ConversionSelector.RequiresConversion(comparands, context.CurrentNode))
        {
            return EquivalencyResult.ContinueWithNext;
        }

        if (comparands.Expectation is null || comparands.Subject is null)
        {
            return EquivalencyResult.ContinueWithNext;
        }

        Type subjectType = comparands.Subject.GetType();
        Type expectationType = comparands.Expectation.GetType();

        if (subjectType.IsSameOrInherits(expectationType))
        {
            return EquivalencyResult.ContinueWithNext;
        }

        if (TryChangeType(comparands.Subject, expectationType, out object convertedSubject))
        {
            context.Tracer.WriteLine(member =>
                Invariant($"Converted subject {comparands.Subject} at {member.Subject} to {expectationType}"));

            comparands.Subject = convertedSubject;
        }
        else
        {
            context.Tracer.WriteLine(member =>
                Invariant($"Subject {comparands.Subject} at {member.Subject} could not be converted to {expectationType}"));
        }

        return EquivalencyResult.ContinueWithNext;
    }

    private static bool TryChangeType(object subject, Type expectationType, out object conversionResult)
    {
        conversionResult = null;

        try
        {
            if (expectationType.IsEnum)
            {
                if (subject is sbyte or byte or short or ushort or int or uint or long or ulong)
                {
                    conversionResult = Enum.ToObject(expectationType, subject);
                    return Enum.IsDefined(expectationType, conversionResult);
                }

                return false;
            }

            conversionResult = Convert.ChangeType(subject, expectationType, CultureInfo.InvariantCulture);
            return true;
        }
        catch (FormatException)
        {
        }
        catch (InvalidCastException)
        {
        }

        return false;
    }

    public override string ToString()
    {
        return string.Empty;
    }
}
