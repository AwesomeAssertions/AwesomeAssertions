using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Exceptions;

public class InnerExceptionSpecs
{
    [Fact]
    public void When_subject_throws_an_exception_with_the_expected_inner_exception_it_should_not_do_anything()
    {
        // Arrange
        Does testSubject = Does.Throw(new Exception("", new ArgumentException()));

        // Act / Assert
        testSubject
            .Invoking(x => x.Do())
            .Should().Throw<Exception>()
            .WithInnerException<ArgumentException>();
    }

    [Fact]
    public void When_subject_throws_an_exception_with_the_expected_inner_base_exception_it_should_not_do_anything()
    {
        // Arrange
        Does testSubject = Does.Throw(new Exception("", new ArgumentNullException()));

        // Act / Assert
        testSubject
            .Invoking(x => x.Do())
            .Should().Throw<Exception>()
            .WithInnerException<ArgumentException>();
    }

    [Fact]
    public void When_subject_throws_an_exception_with_the_expected_inner_exception_from_argument_it_should_not_do_anything()
    {
        // Arrange
        Does testSubject = Does.Throw(new Exception("", new ArgumentException()));

        // Act / Assert
        testSubject
            .Invoking(x => x.Do())
            .Should().Throw<Exception>()
            .WithInnerException(typeof(ArgumentException));
    }

    [Fact]
    public void
        WithInnerExceptionExactly_no_parameters_when_subject_throws_subclass_of_expected_inner_exception_it_should_throw_with_clear_description()
    {
        // Arrange
        var innerException = new ArgumentNullException("InnerExceptionMessage", (Exception)null);

        Action act = () => throw new BadImageFormatException("", innerException);

        try
        {
            // Act
            act.Should().Throw<BadImageFormatException>()
                .WithInnerExceptionExactly<ArgumentException>();

            throw new XunitException("This point should not be reached.");
        }
        catch (XunitException ex)
        {
            // Assert
            ex.Message.Should().Match("Expected*ArgumentException*found*ArgumentNullException*InnerExceptionMessage*");
        }
    }

    [Fact]
    public void WithInnerExceptionExactly_no_parameters_when_subject_throws_expected_inner_exception_it_should_not_do_anything()
    {
        // Arrange
        Action act = () => throw new BadImageFormatException("", new ArgumentNullException());

        // Act / Assert
        act.Should().Throw<BadImageFormatException>()
            .WithInnerExceptionExactly<ArgumentNullException>();
    }

    [Fact]
    public void
        WithInnerExceptionExactly_when_subject_throws_subclass_of_expected_inner_exception_it_should_throw_with_clear_description()
    {
        // Arrange
        var innerException = new ArgumentNullException("InnerExceptionMessage", (Exception)null);

        Action act = () => throw new BadImageFormatException("", innerException);

        try
        {
            // Act
            act.Should().Throw<BadImageFormatException>()
                .WithInnerExceptionExactly<ArgumentException>("we want to test the {0} message", "failure");

            throw new XunitException("This point should not be reached.");
        }
        catch (XunitException ex)
        {
            // Assert
            ex.Message.Should()
                .Match("Expected*ArgumentException*because*failure message*ArgumentNullException*InnerExceptionMessage*");
        }
    }

    [Fact]
    public void
        WithInnerExceptionExactly_with_type_exception_when_subject_throws_expected_inner_exception_it_should_not_do_anything()
    {
        // Arrange
        Action act = () => throw new BadImageFormatException("", new ArgumentNullException());

        // Act / Assert
        act.Should().Throw<BadImageFormatException>()
            .WithInnerExceptionExactly(typeof(ArgumentNullException), "the parameter is {0} relevant for this test", "parameter");
    }

    [Fact]
    public void
        WithInnerExceptionExactly_with_type_exception_no_parameters_when_subject_throws_expected_inner_exception_it_should_not_do_anything()
    {
        // Arrange
        Action act = () => throw new BadImageFormatException("", new ArgumentNullException());

        // Act / Assert
        act.Should().Throw<BadImageFormatException>()
            .WithInnerExceptionExactly(typeof(ArgumentNullException));
    }

    [Fact]
    public void
        WithInnerExceptionExactly_with_type_exception_when_subject_throws_subclass_of_expected_inner_exception_it_should_throw_with_clear_description()
    {
        // Arrange
        var innerException = new ArgumentNullException("InnerExceptionMessage", (Exception)null);

        Action act = () => throw new BadImageFormatException("", innerException);

        try
        {
            // Act
            act.Should().Throw<BadImageFormatException>()
                .WithInnerExceptionExactly(typeof(ArgumentException), "we want to test the {0} message", "failure");

            throw new XunitException("This point should not be reached.");
        }
        catch (XunitException ex)
        {
            // Assert
            ex.Message.Should()
                .Match("Expected*ArgumentException*because*failure message*ArgumentNullException*InnerExceptionMessage*");
        }
    }

    [Fact]
    public void WithInnerExceptionExactly_when_subject_throws_expected_inner_exception_it_should_not_do_anything()
    {
        // Arrange
        Action act = () => throw new BadImageFormatException("", new ArgumentNullException());

        // Act / Assert
        act.Should().Throw<BadImageFormatException>()
            .WithInnerExceptionExactly<ArgumentNullException>("the parameter is {0} relevant for this test", "parameter");
    }

    [Fact]
    public void An_exception_without_the_expected_inner_exception_has_a_descriptive_message()
    {
        // Arrange
        Action subject = () => throw new BadImageFormatException("");

        // Act
        Action act = () => subject.Should().Throw<BadImageFormatException>()
            .WithInnerExceptionExactly<ArgumentNullException>("we want to test the {0} message", "failure");

        // Assert
        act.Should().Throw<XunitException>()
            .WithMessage("*we want to test the failure message*no inner exception*");
    }

    [Fact]
    public void When_subject_throws_an_exception_with_an_unexpected_inner_exception_it_should_throw_with_clear_description()
    {
        // Arrange
        var innerException = new NullReferenceException("InnerExceptionMessage");

        Does testSubject = Does.Throw(new Exception("", innerException));

        try
        {
            // Act
            testSubject
                .Invoking(x => x.Do())
                .Should().Throw<Exception>()
                .WithInnerException<ArgumentException>("we want to test the {0} message", "failure");

            throw new XunitException("This point should not be reached");
        }
        catch (XunitException exc)
        {
            // Assert
            exc.Message.Should().Match(
                "Expected*ArgumentException*because*failure message*NullReferenceException*InnerExceptionMessage*");
        }
    }

    [Fact]
    public void When_subject_throws_an_exception_without_expected_inner_exception_it_should_throw_with_clear_description()
    {
        try
        {
            Does testSubject = Does.Throw<Exception>();

            testSubject.Invoking(x => x.Do()).Should().Throw<Exception>()
                .WithInnerException<InvalidOperationException>("we want to test the {0} message", "failure");

            throw new XunitException("This point should not be reached");
        }
        catch (XunitException ex)
        {
            ex.Message.Should().Be(
                "Expected inner System.InvalidOperationException because we want to test the failure message,"
                + " but the thrown exception has no inner exception.");
        }
    }

    [Fact]
    public void When_an_inner_exception_matches_exactly_it_should_allow_chaining_more_asserts_on_that_exception_type()
    {
        // Act
        Action act = () =>
            throw new ArgumentException("OuterMessage", new InvalidOperationException("InnerMessage"));

        // Assert
        act
            .Should().ThrowExactly<ArgumentException>()
            .WithInnerExceptionExactly<InvalidOperationException>()
            .Where(i => i.Message == "InnerMessage");
    }

    [Fact]
    public void
        When_an_inner_exception_matches_exactly_it_should_allow_chaining_more_asserts_on_that_exception_type_from_argument()
    {
        // Act
        Action act = () =>
            throw new ArgumentException("OuterMessage", new InvalidOperationException("InnerMessage"));

        // Assert
        act
            .Should().ThrowExactly<ArgumentException>()
            .WithInnerExceptionExactly(typeof(InvalidOperationException))
            .Where(i => i.Message == "InnerMessage");
    }

    [Fact]
    public void When_injecting_a_null_predicate_it_should_throw()
    {
        // Arrange
        Action act = () => throw new Exception();

        // Act
        Action act2 = () => act.Should().Throw<Exception>()
            .Where(exceptionExpression: null);

        // Act
        act2.Should().ThrowExactly<ArgumentNullException>()
            .WithParameterName("exceptionExpression");
    }
}
