using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Numeric;

public partial class NullableNumericAssertionSpecs
{
    public class Match
    {
        [Fact]
        public void When_nullable_value_satisfies_predicate_it_should_not_throw()
        {
            int? nullableInteger = 1;

            nullableInteger.Should().Match(o => o.HasValue);
        }

        [Fact]
        public void When_nullable_value_does_not_match_the_predicate_it_should_throw()
        {
            int? nullableInteger = 1;

            Action act = () =>
                nullableInteger.Should().Match(o => !o.HasValue, "we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>()
                .WithMessage(
                    "Expected value to match Not(o.HasValue) because we want to test the failure message, but found 1.");
        }

        [Fact]
        public void When_nullable_value_is_matched_against_a_null_it_should_throw()
        {
            int? nullableInteger = 1;

            Action act = () => nullableInteger.Should().Match(null);

            act.Should().ThrowExactly<ArgumentNullException>()
                .WithParameterName("predicate");
        }
    }
}
