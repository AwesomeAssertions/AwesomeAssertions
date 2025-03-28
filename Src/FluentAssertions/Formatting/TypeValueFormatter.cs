using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FluentAssertions.Formatting;

public sealed class TypeValueFormatter : IValueFormatter
{
    public bool CanHandle(object value)
    {
        return value is Type or ShortTypeValue;
    }

    public void Format(object value, FormattedObjectGraph formattedGraph, FormattingContext context, FormatChild formatChild)
    {
        if (value is ShortTypeValue shortValue)
        {
            FormatType(shortValue.Type, formattedGraph.AddFragment, withNamespaces: false);
        }
        else
        {
            FormatType((Type)value, formattedGraph.AddFragment);
        }
    }

    /// <summary>
    /// Format a type to a friendly name.
    /// </summary>
    /// <param name="type">The type to format.</param>
    /// <returns>The friendly type name.</returns>
    internal static string Format(Type type)
    {
        StringBuilder sb = new();
        FormatType(type, value => sb.Append(value));
        return sb.ToString();
    }

    /// <summary>
    /// Format a type to a friendly name.
    /// </summary>
    /// <param name="type">The type to format.</param>
    /// <param name="append">Function for appending the formatted type part</param>
    /// <param name="withNamespaces">
    /// If true, all types, including inner types of generic arguments, are formatted
    /// using their full namespace.
    /// </param>
    internal static void FormatType(Type type, Action<string> append, bool withNamespaces = true)
    {
        if (Nullable.GetUnderlyingType(type) is Type nullbase)
        {
            FormatType(nullbase, append, withNamespaces);
            append("?");
        }
        else if (type.IsGenericType)
        {
            if (withNamespaces && !string.IsNullOrEmpty(type.Namespace))
            {
                append(type.Namespace);
                append(".");
            }

            FormatGenericType(type, append, withNamespaces);
        }
        else if (type.BaseType == typeof(Array))
        {
            FormatArrayType(type, append, withNamespaces);
        }
        else if (LanguageKeywords.TryGetValue(type, out string alias))
        {
            append(alias);
        }
        else if (withNamespaces)
        {
            append(type.FullName);
        }
        else
        {
            append(type.Name);
        }
    }

    /// <summary>
    /// Format an array type of any dimension.
    /// </summary>
    /// <param name="type">The array type. Can have any dimension.</param>
    /// <param name="append">Function for appending the formatted type part.</param>
    /// <param name="withNamespace">If true, the type is formatted with its full namespace.</param>
    private static void FormatArrayType(Type type, Action<string> append, bool withNamespace)
    {
        FormatType(type.GetElementType(), append, withNamespace);

        append("[");
        append(new string(',', type.GetArrayRank() - 1));
        append("]");
    }

    /// <summary>
    /// Format a bound or unbound generic type.
    /// </summary>
    /// <param name="type">The generic type. Can be bound or unbound generic.</param>
    /// <param name="append">Function for appending the formatted type part.</param>
    /// <param name="withNamespace">If true, the type is formatted with its full namespace.</param>
    private static void FormatGenericType(Type type, Action<string> append, bool withNamespace)
    {
        FormatDeclaringTypeNames(type, append);

        append(type.Name[..type.Name.LastIndexOf('`')]);
        append("<");

        FormatGenericArguments(type, append, withNamespace);

        append(">");
    }

    /// <summary>
    /// Format generic arguments of a type.
    /// </summary>
    /// <param name="type">The generic base type, of which the generic arguments are formatted.</param>
    /// <param name="append">Function for appending the formatted type part.</param>
    /// <param name="withNamespace">If true, the type is formatted with its full namespace.</param>
    private static void FormatGenericArguments(Type type, Action<string> append, bool withNamespace)
    {
        Type[] types = type.GetGenericArguments();
        bool isUnboundType = type.ContainsGenericParameters;

        int numberOfGenericArguments = GetNumberOfGenericArguments(type);
        int firstGenericArgumentIndex = types.Length - numberOfGenericArguments;
        for (int index = firstGenericArgumentIndex; index < types.Length; index++)
        {
            if (isUnboundType)
            {
                append(types[index].Name);
            }
            else
            {
                FormatType(types[index], append, withNamespace);
            }

            if (index < types.Length - 1)
            {
                append(", ");
            }
        }
    }

    /// <summary>
    /// Get number of generic arguments of a type.
    /// </summary>
    /// <remarks>
    /// For nested generic classes like class A&lt;T&gt; { class B&lt;T2&gt; { } },
    /// the inner class also holds the generic arguments of all parent classes,
    /// which we don't want for the formatting.
    /// </remarks>
    /// <param name="type">The generic type of which to get the generic arguments.</param>
    /// <returns>The count of generic arguments which are associated only with <paramref name="type"/>.</returns>
    private static int GetNumberOfGenericArguments(Type type) =>
        int.Parse(type.Name[(type.Name.LastIndexOf('`') + 1)..], CultureInfo.InvariantCulture);

    /// <summary>
    /// Format all declaring type names of type <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type, of which to format the declaring types.</param>
    /// <param name="append">Function for appending the formatted type part.</param>
    private static void FormatDeclaringTypeNames(Type type, Action<string> append)
    {
        foreach (Type declaringType in GetDeclaringTypes(type))
        {
            // for the declaring types we stick to the default, short notation like Dictionary`2.
            append(declaringType.Name);
            append("+");
        }
    }

    /// <summary>
    /// Get all declaring types, ordered from outer to inner.
    /// </summary>
    /// <param name="type">The base type</param>
    /// <returns>All declaring types</returns>
    private static List<Type> GetDeclaringTypes(Type type)
    {
        if (type.DeclaringType is null)
        {
            return [];
        }

        // We don't iterate from inner to outer, because that wouldn't work
        // with an append, but would require an insert, too.
        List<Type> declaringTypes = [];
        Type declaringType = type.DeclaringType;
        while (declaringType is not null)
        {
            declaringTypes.Insert(0, declaringType);
            declaringType = declaringType.DeclaringType;
        }

        return declaringTypes;
    }

    /// <summary>
    /// Lookup dictionary to use language keyword instead of type name.
    /// </summary>
    private static readonly Dictionary<Type, string> LanguageKeywords = new()
    {
        { typeof(bool), "bool" },
        { typeof(byte), "byte" },
        { typeof(char), "char" },
        { typeof(decimal), "decimal" },
        { typeof(double), "double" },
        { typeof(float), "float" },
        { typeof(int), "int" },
        { typeof(long), "long" },
        { typeof(object), "object" },
        { typeof(sbyte), "sbyte" },
        { typeof(short), "short" },
        { typeof(string), "string" },
        { typeof(uint), "uint" },
        { typeof(ulong), "ulong" },
        { typeof(void), "void" },
    };
}
