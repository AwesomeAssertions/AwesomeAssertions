using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Types;

public sealed class ParameterInfoAssertionSpecs
{
    public sealed class BeDecoratedWith
    {
        [Fact]
        public void When_parameter_is_decorated_with_expected_attribute_it_succeeds()
        {
            ParameterInfo subject = typeof(TestStub).GetMethod(nameof(TestStub.WithAnnotatedParameter)).GetParameters()[0];

            subject.Should().BeDecoratedWith<MaybeNullWhenAttribute>();
        }

        [Fact]
        public void When_parameter_is_decorated_with_expected_attribute_with_the_expected_properties_it_succeeds()
        {
            ParameterInfo subject = typeof(TestStub).GetMethod(nameof(TestStub.WithAnnotatedParameter)).GetParameters()[0];

            subject.Should().BeDecoratedWith<MaybeNullWhenAttribute>(x => !x.ReturnValue);
        }

        [Fact]
        public void When_input_parameter_is_not_decorated_with_expected_attribute_it_fails()
        {
            ParameterInfo subject = typeof(TestStub).GetMethod(nameof(TestStub.WithoutAnnotations)).GetParameters()[0];

            Action act = () => subject.Should().BeDecoratedWith<MaybeNullWhenAttribute>("we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>()
                .WithMessage("Expected parameter object input *MaybeNullWhenAttribute because *failure message*not found*");
        }

        [Fact]
        public void When_return_parameter_is_not_decorated_with_expected_attribute_it_fails()
        {
            ParameterInfo subject = typeof(TestStub).GetMethod(nameof(TestStub.WithoutAnnotations)).ReturnParameter;

            Action act = () => subject.Should().BeDecoratedWith<NotNullAttribute>("we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>()
                .WithMessage("Expected return parameter bool *NotNullAttribute because *failure message*not found*");
        }

        [Fact]
        public void When_parameter_is_decorated_with_expected_attribute_that_has_unexpected_properties_it_fails()
        {
            ParameterInfo subject = typeof(TestStub).GetMethod(nameof(TestStub.WithAnnotatedParameter)).GetParameters()[0];

            Action act = () => subject.Should().BeDecoratedWith<MaybeNullWhenAttribute>(x => x.ReturnValue,
                "we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>()
                .WithMessage("Expected*parameter System.Object&*MaybeNullWhenAttribute matching *ReturnValue* because*failure message*no attribute*was found*");
        }

        [Fact]
        public void When_injecting_a_null_predicate_it_should_throw()
        {
            ParameterInfo subject = typeof(TestStub).GetMethod(nameof(TestStub.WithAnnotatedParameter)).GetParameters()[0];

            Action act = () => subject.Should().BeDecoratedWith<MaybeNullWhenAttribute>(isMatchingAttributePredicate: null);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("isMatchingAttributePredicate");
        }

        [Fact]
        public void When_parameter_is_null_it_fails()
        {
            ParameterInfo subject = null;

            Action act = () => subject.Should().BeDecoratedWith<MaybeNullWhenAttribute>();

            act.Should().Throw<XunitException>()
                .WithMessage("Expected parameter*MaybeNullWhenAttribute*subject is <null>*");
        }
    }

    public sealed class NotBeDecoratedWith
    {
        [Fact]
        public void When_parameter_is_not_decorated_with_expected_attribute_it_succeeds()
        {
            ParameterInfo subject = typeof(TestStub).GetMethod(nameof(TestStub.WithoutAnnotations)).GetParameters()[0];

            subject.Should().NotBeDecoratedWith<MaybeNullWhenAttribute>();
        }

        [Fact]
        public void When_parameter_is_decorated_with_expected_attribute_with_the_expected_properties_it_fails()
        {
            ParameterInfo subject = typeof(TestStub).GetMethod(nameof(TestStub.WithAnnotatedParameter)).GetParameters()[0];

            Action act = () => subject.Should().NotBeDecoratedWith<MaybeNullWhenAttribute>(x => !x.ReturnValue,
                "we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>()
                .WithMessage("Did not expect parameter System.Object& input to be decorated with *.MaybeNullWhenAttribute matching Not(x.ReturnValue) because *failure message*was found*");
        }

        [Fact]
        public void When_input_parameter_is_decorated_with_expected_attribute_it_fails()
        {
            ParameterInfo subject = typeof(TestStub).GetMethod(nameof(TestStub.WithAnnotatedParameter)).GetParameters()[0];

            Action act = () => subject.Should().NotBeDecoratedWith<MaybeNullWhenAttribute>("we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>()
                .WithMessage("Did not expect parameter System.Object& input *MaybeNullWhenAttribute because *failure message*was found*");
        }

        [Fact]
        public void When_return_parameter_is_decorated_with_expected_attribute_it_fails()
        {
            ParameterInfo subject = typeof(TestStub).GetMethod(nameof(TestStub.ReturnsNotNull)).ReturnParameter;

            Action act = () => subject.Should().NotBeDecoratedWith<NotNullAttribute>("we want to test the {0} message", "failure");

            act.Should().Throw<XunitException>()
                .WithMessage("Did not expect return parameter object *NotNullAttribute because *failure message*was found*");
        }

        [Fact]
        public void When_parameter_is_decorated_with_expected_attribute_that_has_unexpected_properties_it_succeeds()
        {
            ParameterInfo subject = typeof(TestStub).GetMethod(nameof(TestStub.WithAnnotatedParameter)).GetParameters()[0];

            subject.Should().NotBeDecoratedWith<MaybeNullWhenAttribute>(x => x.ReturnValue);
        }

        [Fact]
        public void When_injecting_a_null_predicate_it_should_throw()
        {
            ParameterInfo subject = typeof(TestStub).GetMethod(nameof(TestStub.WithAnnotatedParameter)).GetParameters()[0];

            Action act = () => subject.Should().NotBeDecoratedWith<MaybeNullWhenAttribute>(isMatchingAttributePredicate: null);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("isMatchingAttributePredicate");
        }

        [Fact]
        public void When_parameter_is_null_it_fails()
        {
            ParameterInfo subject = null;

            Action act = () => subject.Should().NotBeDecoratedWith<MaybeNullWhenAttribute>();

            act.Should().Throw<XunitException>()
                .WithMessage("Did not expect parameter*MaybeNullWhenAttribute*subject is <null>*");
        }
    }

    private static class TestStub
    {
#pragma warning disable AV1562 // Don't use `ref` or `out` parameters: Used as testing subject.
        public static bool WithAnnotatedParameter([MaybeNullWhen(false)] out object input)
        {
            input = null;
            return false;
        }

        public static bool WithoutAnnotations(object input) => input is null;

        [return: NotNull]
        public static object ReturnsNotNull() => new();
    }
}
