Imports AwesomeAssertions
Imports Xunit
Imports Xunit.Sdk

<CodeAnalysis.SuppressMessage("Naming rules", "CA1707:Identifiers should not contain underscores", MessageId:="Keeping this as in C#")>
Public Class VBSpecs
    <Fact>
    Public Sub Caller_identification_works_with_parentheses()
        ' Arrange
        Const subject = False

        ' Act
        Dim act As Action = Sub() subject.Should().BeTrue()

        ' Assert
        act.Should().Throw(Of XunitException).WithMessage("Expected subject to be true, but found false.")
    End Sub

    <Fact>
    Public Sub Caller_identification_works_without_parentheses()
        ' Arrange
        Const subject = False

        ' Act
        Dim act As Action = Sub() subject.Should.BeTrue()

        ' Assert
        act.Should().Throw(Of XunitException).WithMessage("Expected subject to be true, but found false.")
    End Sub
End Class
