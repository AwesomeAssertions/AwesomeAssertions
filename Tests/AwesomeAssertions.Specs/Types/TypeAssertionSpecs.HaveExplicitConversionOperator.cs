using System;
using AwesomeAssertions.Common;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Types;

/// <content>
/// The [Not]HaveExplicitConversionOperator specs.
/// </content>
public partial class TypeAssertionSpecs
{
    public class HaveExplicitConversionOperator
    {
        [Fact]
        public void When_asserting_a_type_has_an_explicit_conversion_operator_which_it_does_it_succeeds()
        {
            // Arrange
            var type = typeof(TypeWithConversionOperators);
            var sourceType = typeof(TypeWithConversionOperators);
            var targetType = typeof(byte);

            // Act / Assert
            type.Should()
                .HaveExplicitConversionOperator(sourceType, targetType)
                .Which.Should()
                .NotBeNull();
        }

        [Fact]
        public void Can_chain_an_additional_assertion_on_the_implicit_conversion_operator()
        {
            // Arrange
            var type = typeof(TypeWithConversionOperators);
            var sourceType = typeof(TypeWithConversionOperators);
            var targetType = typeof(byte);

            // Act
            Action act = () => type
                .Should().HaveExplicitConversionOperator(sourceType, targetType)
                .Which.Should().HaveAccessModifier(CSharpAccessModifier.Private);

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage(
                    "Expected method explicit operator byte(TypeWithConversionOperators) to be Private, but it is Public.");
        }

        [Fact]
        public void When_asserting_a_type_has_an_explicit_conversion_operator_which_it_does_not_it_fails_with_a_useful_message()
        {
            // Arrange
            var type = typeof(TypeWithConversionOperators);
            var sourceType = typeof(TypeWithConversionOperators);
            var targetType = typeof(string);

            // Act
            Action act = () =>
                type.Should().HaveExplicitConversionOperator(
                    sourceType, targetType, "we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage(
                    "Expected public static explicit string(*.TypeWithConversionOperators) to exist *failure message*" +
                    ", but it does not.");
        }

        [Fact]
        public void When_subject_is_null_have_explicit_conversion_operator_should_fail()
        {
            // Arrange
            Type type = null;

            // Act
            Action act = () =>
                type.Should().HaveExplicitConversionOperator(
                    typeof(TypeWithConversionOperators), typeof(string), "we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage(
                    "Expected public static explicit string(*.TypeWithConversionOperators) to exist *failure message*" +
                    ", but type is <null>.");
        }

        [Fact]
        public void When_asserting_a_type_has_an_explicit_conversion_operator_from_null_it_should_throw()
        {
            // Arrange
            var type = typeof(TypeWithConversionOperators);

            // Act
            Action act = () =>
                type.Should().HaveExplicitConversionOperator(null, typeof(string));

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>()
                .WithParameterName("sourceType");
        }

        [Fact]
        public void When_asserting_a_type_has_an_explicit_conversion_operator_to_null_it_should_throw()
        {
            // Arrange
            var type = typeof(TypeWithConversionOperators);

            // Act
            Action act = () =>
                type.Should().HaveExplicitConversionOperator(typeof(TypeWithConversionOperators), null);

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>()
                .WithParameterName("targetType");
        }
    }

    public class HaveExplicitConversionOperatorOfT
    {
        [Fact]
        public void When_asserting_a_type_has_an_explicit_conversion_operatorOfT_which_it_does_it_succeeds()
        {
            // Arrange
            var type = typeof(TypeWithConversionOperators);

            // Act / Assert
            type.Should()
                .HaveExplicitConversionOperator<TypeWithConversionOperators, byte>()
                .Which.Should()
                .NotBeNull();
        }

        [Fact]
        public void When_asserting_a_type_has_an_explicit_conversion_operatorOfT_which_it_does_not_it_fails()
        {
            // Arrange
            var type = typeof(TypeWithConversionOperators);

            // Act
            Action act = () =>
                type.Should().HaveExplicitConversionOperator<TypeWithConversionOperators, string>(
                    "we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage(
                    "Expected public static explicit string(*.TypeWithConversionOperators) to exist *failure message*" +
                    ", but it does not.");
        }

        [Fact]
        public void When_subject_is_null_have_explicit_conversion_operatorOfT_should_fail()
        {
            // Arrange
            Type type = null;

            // Act
            Action act = () =>
                type.Should().HaveExplicitConversionOperator<TypeWithConversionOperators, string>(
                    "we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage(
                    "Expected public static explicit string(*.TypeWithConversionOperators) to exist *failure message*" +
                    ", but type is <null>.");
        }
    }

    public class NotHaveExplicitConversionOperator
    {
        [Fact]
        public void When_asserting_a_type_does_not_have_an_explicit_conversion_operator_which_it_does_not_it_succeeds()
        {
            // Arrange
            var type = typeof(TypeWithConversionOperators);
            var sourceType = typeof(TypeWithConversionOperators);
            var targetType = typeof(string);

            // Act / Assert
            type.Should()
                .NotHaveExplicitConversionOperator(sourceType, targetType);
        }

        [Fact]
        public void When_asserting_a_type_does_not_have_an_explicit_conversion_operator_which_it_does_it_fails()
        {
            // Arrange
            var type = typeof(TypeWithConversionOperators);
            var sourceType = typeof(TypeWithConversionOperators);
            var targetType = typeof(byte);

            // Act
            Action act = () =>
                type.Should().NotHaveExplicitConversionOperator(
                    sourceType, targetType, "we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage(
                    "Expected public static explicit byte(*.TypeWithConversionOperators) to not exist *failure message*" +
                    ", but it does.");
        }

        [Fact]
        public void When_subject_is_null_not_have_explicit_conversion_operator_should_fail()
        {
            // Arrange
            Type type = null;

            // Act
            Action act = () =>
                type.Should().NotHaveExplicitConversionOperator(
                    typeof(TypeWithConversionOperators), typeof(string), "we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage(
                    "Expected public static explicit string(*.TypeWithConversionOperators) to not exist *failure message*" +
                    ", but type is <null>.");
        }

        [Fact]
        public void When_asserting_a_type_does_not_have_an_explicit_conversion_operator_from_null_it_should_throw()
        {
            // Arrange
            var type = typeof(TypeWithConversionOperators);

            // Act
            Action act = () =>
                type.Should().NotHaveExplicitConversionOperator(null, typeof(string));

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>()
                .WithParameterName("sourceType");
        }

        [Fact]
        public void When_asserting_a_type_does_not_have_an_explicit_conversion_operator_to_null_it_should_throw()
        {
            // Arrange
            var type = typeof(TypeWithConversionOperators);

            // Act
            Action act = () =>
                type.Should().NotHaveExplicitConversionOperator(typeof(TypeWithConversionOperators), null);

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>()
                .WithParameterName("targetType");
        }
    }

    public class NotHaveExplicitConversionOperatorOfT
    {
        [Fact]
        public void When_asserting_a_type_does_not_have_an_explicit_conversion_operatorOfT_which_it_does_not_it_succeeds()
        {
            // Arrange
            var type = typeof(TypeWithConversionOperators);

            // Act / Assert
            type.Should()
                .NotHaveExplicitConversionOperator<TypeWithConversionOperators, string>();
        }

        [Fact]
        public void When_asserting_a_type_does_not_have_an_explicit_conversion_operatorOfT_which_it_does_it_fails()
        {
            // Arrange
            var type = typeof(TypeWithConversionOperators);

            // Act
            Action act = () =>
                type.Should().NotHaveExplicitConversionOperator<TypeWithConversionOperators, byte>(
                    "we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage(
                    "Expected public static explicit byte(*.TypeWithConversionOperators) to not exist *failure message*" +
                    ", but it does.");
        }
    }
}
