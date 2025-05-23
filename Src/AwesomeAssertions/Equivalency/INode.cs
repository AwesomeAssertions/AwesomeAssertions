using System;

namespace AwesomeAssertions.Equivalency;

/// <summary>
/// Represents a node in the object graph that is being compared as part of a structural equivalency check.
/// This can be the root object, a collection item, a dictionary element, a property or a field.
/// </summary>
public interface INode
{
    /// <summary>
    /// The name of the variable on which a structural equivalency assertion is executed or
    /// the default if reflection failed.
    /// </summary>
    GetSubjectId GetSubjectId { get; }

    /// <summary>
    /// Gets the type of this node, e.g. the type of the field or property, or the type of the collection item.
    /// </summary>
    Type Type { get; }

    /// <summary>
    /// Gets the type of the parent node, e.g. the type that declares a property or field.
    /// </summary>
    /// <value>
    /// Is <see langword="null"/> for the root object.
    /// </value>
    Type ParentType { get; }

    /// <summary>
    /// Gets the path from the root of the subject upto and including the current node.
    /// </summary>
    Pathway Subject { get; internal set; }

    /// <summary>
    /// Gets the path from the root of the expectation upto and including the current node.
    /// </summary>
    Pathway Expectation { get; }

    /// <summary>
    /// Gets a zero-based number representing the depth within the object graph
    /// </summary>
    /// <remarks>
    /// The root object has a depth of <c>0</c>, the next nested object a depth of <c>1</c>, etc.
    /// See also <a href="https://www.geeksforgeeks.org/height-and-depth-of-a-node-in-a-binary-tree/">this article</a>
    /// </remarks>
    int Depth { get; }

    /// <summary>
    /// Gets a value indicating whether the current node is the root.
    /// </summary>
    bool IsRoot { get; }

    /// <summary>
    /// Gets a value indicating if the root of this graph is a collection.
    /// </summary>
    bool RootIsCollection { get; }

    /// <summary>
    /// Adjusts the current node to reflect a remapped subject member during a structural equivalency check.
    /// Ensures that assertion failures report the correct subject member name when the matching process selects
    /// a different member in the subject compared to the expectation.
    /// </summary>
    /// <param name="subjectMember">
    /// The specific member in the subject that the current node should be remapped to.
    /// </param>
    void AdjustForRemappedSubject(IMember subjectMember);
}
