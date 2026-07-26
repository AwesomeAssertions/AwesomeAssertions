using System;
using System.Reflection;
using AwesomeAssertions.Common;

namespace AwesomeAssertions.Equivalency;

/// <summary>
/// Provides factory methods for creating <see cref="IMember"/> instances that wrap the fields and properties of an object graph.
/// </summary>
public static class MemberFactory
{
    /// <summary>
    /// Creates an <see cref="IMember"/> representing the field or property described by <paramref name="memberInfo"/>.
    /// </summary>
    /// <param name="memberInfo">The reflection metadata of the field or property to wrap.</param>
    /// <param name="parent">The node representing the object that declares the member.</param>
    /// <returns>An <see cref="IMember"/> wrapping the specified field or property.</returns>
    /// <exception cref="NotSupportedException">
    /// <paramref name="memberInfo"/> represents a member that is neither a field nor a property.
    /// </exception>
    public static IMember Create(MemberInfo memberInfo, INode parent)
    {
        return memberInfo.MemberType switch
        {
            MemberTypes.Field => new Field((FieldInfo)memberInfo, parent),
            MemberTypes.Property => new Property((PropertyInfo)memberInfo, parent),
            _ => throw new NotSupportedException($"Don't know how to deal with a {memberInfo.MemberType}")
        };
    }

    internal static IMember Find(object target, string memberName, INode parent)
    {
        PropertyInfo property = target.GetType().FindProperty(memberName, MemberVisibility.Public | MemberVisibility.ExplicitlyImplemented);

        if (property is not null && !property.IsIndexer())
        {
            return new Property(property, parent);
        }

        FieldInfo field = target.GetType().FindField(memberName, MemberVisibility.Public);
        return field is not null ? new Field(field, parent) : null;
    }
}
