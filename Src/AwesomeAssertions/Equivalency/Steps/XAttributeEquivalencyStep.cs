﻿using System.Xml.Linq;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Equivalency.Steps;

public class XAttributeEquivalencyStep : EquivalencyStep<XAttribute>
{
    protected override EquivalencyResult OnHandle(Comparands comparands,
        IEquivalencyValidationContext context,
        IValidateChildNodeEquivalency nestedValidator)
    {
        var subject = (XAttribute)comparands.Subject;
        var expectation = (XAttribute)comparands.Expectation;

        AssertionChain.GetOrCreate().For(context).ReuseOnce();

        subject.Should().Be(expectation, context.Reason.FormattedMessage, context.Reason.Arguments);

        return EquivalencyResult.EquivalencyProven;
    }
}
