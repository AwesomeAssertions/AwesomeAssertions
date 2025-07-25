using System;
using AwesomeAssertions.Common;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Types;

/// <content>
/// The [Not]HaveConstructor specs.
/// </content>
public partial class TypeAssertionSpecs
{
    public class HaveConstructor
    {
        [Fact]
        public void When_asserting_a_type_has_a_constructor_which_it_does_it_succeeds()
        {
            // Arrange
            var type = typeof(ClassWithMembers);

            // Act / Assert
            type.Should()
                .HaveConstructor([typeof(string)])
                .Which.Should().HaveAccessModifier(CSharpAccessModifier.Private);
        }

        [Fact]
        public void When_asserting_a_type_has_a_constructor_which_it_does_not_it_fails()
        {
            // Arrange
            var type = typeof(ClassWithNoMembers);

            // Act
            Action act = () =>
                type.Should().HaveConstructor([typeof(int), typeof(Type)], "we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage(
                    "Expected constructor *ClassWithNoMembers(int, System.Type) to exist *failure message*" +
                    ", but it does not.");
        }

        [Fact]
        public void When_subject_is_null_have_constructor_should_fail()
        {
            // Arrange
            Type type = null;

            // Act
            Action act = () =>
                type.Should().HaveConstructor([typeof(string)], "we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected constructor type(string) to exist *failure message*, but type is <null>.");
        }

        [Fact]
        public void When_asserting_a_type_has_a_constructor_of_null_it_should_throw()
        {
            // Arrange
            Type type = typeof(ClassWithMembers);

            // Act
            Action act = () =>
                type.Should().HaveConstructor(null);

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>()
                .WithParameterName("parameterTypes");
        }
    }

    public class NotHaveConstructor
    {
        [Fact]
        public void When_asserting_a_type_does_not_have_a_constructor_which_it_does_not_it_succeeds()
        {
            // Arrange
            var type = typeof(ClassWithNoMembers);

            // Act / Assert
            type.Should()
                .NotHaveConstructor([typeof(string)]);
        }

        [Fact]
        public void When_asserting_a_type_does_not_have_a_constructor_which_it_does_it_fails()
        {
            // Arrange
            var type = typeof(ClassWithMembers);

            // Act
            Action act = () =>
                type.Should().NotHaveConstructor([typeof(string)], "we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage(
                    "Expected constructor *.ClassWithMembers(string) not to exist *failure message*, but it does.");
        }

        [Fact]
        public void When_subject_is_null_not_have_constructor_should_fail()
        {
            // Arrange
            Type type = null;

            // Act
            Action act = () =>
                type.Should().NotHaveConstructor([typeof(string)], "we want to test the failure {0}", "message");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected constructor type(string) not to exist *failure message*, but type is <null>.");
        }

        [Fact]
        public void When_asserting_a_type_does_not_have_a_constructor_of_null_it_should_throw()
        {
            // Arrange
            Type type = typeof(ClassWithMembers);

            // Act
            Action act = () =>
                type.Should().NotHaveConstructor(null);

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>()
                .WithParameterName("parameterTypes");
        }
    }
}
