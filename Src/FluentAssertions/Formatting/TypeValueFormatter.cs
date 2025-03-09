using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FluentAssertions.Formatting
{
    public class TypeValueFormatter : IValueFormatter
    {
        public bool CanHandle(object value)
        {
            return value is Type;
        }

        public void Format(object value, FormattedObjectGraph formattedGraph, FormattingContext context, FormatChild formatChild)
        {
            Type type = (Type)value;
            FormatType(type, formattedGraph.AddFragment);
        }

        /// <summary>
        /// Format a type to a friendly name
        /// </summary>
        /// <param name="type">The type to format</param>
        /// <returns>The friendly type name</returns>
        public static string Format(Type type)
        {
            if (type is null)
            {
                return "<null>";
            }

            StringBuilder sb = new();
            FormatType(type, value => sb.Append(value), withLeadingNamespace: true);
            return sb.ToString();
        }

        /// <summary>
        /// Format a type to a friendly name.
        /// </summary>
        /// <param name="type">The type to format.</param>
        /// <param name="sb">The target string builder.</param>
        public static void Format(Type type, StringBuilder sb) =>
            FormatType(type, value => sb.Append(value), withLeadingNamespace: true);

        /// <summary>
        /// Format a type to a friendly name without its leading namespace.
        /// </summary>
        /// <remarks>
        /// Namespaces of other types, e.g. generic arguments, are still added.
        /// </remarks>
        /// <param name="type">The type to format.</param>
        /// <returns>The friendly type name.</returns>
        public static string FormatWithoutLeadingNamespace(Type type)
        {
            if (type is null)
            {
                return "<null>";
            }

            StringBuilder sb = new();
            FormatType(type, value => sb.Append(value), withLeadingNamespace: false);
            return sb.ToString();
        }

        /// <summary>
        /// Format a type to a friendly name without its leading namespace.
        /// </summary>
        /// <remarks>
        /// Namespaces of other types, e.g. generic arguments, are still added.
        /// </remarks>
        /// <param name="type">The type to format</param>
        /// <param name="sb">The target string builder</param>
        public static void FormatWithoutLeadingNamespace(Type type, StringBuilder sb) =>
            FormatType(type, value => sb.Append(value), withLeadingNamespace: false);

        /// <summary>
        /// Format a type to a friendly name.
        /// </summary>
        /// <param name="type">The type to format</param>
        /// <param name="append">Function for appending the formatted type part</param>
        /// <param name="withLeadingNamespace">
        ///     If true, the initial type name is formatted with its full namespace.
        ///     Inner types, e.g. of generic arguments, are always formatted with their full namespaces.
        /// </param>
        private static void FormatType(Type type, Action<string> append, bool withLeadingNamespace = true)
        {
            if (type is null)
            {
                append("<null>");
            }
            else if (Nullable.GetUnderlyingType(type) is Type nullbase)
            {
                FormatType(nullbase, append, withLeadingNamespace);
                append("?");
            }
            else if (type.IsGenericType)
            {
                if (withLeadingNamespace && !string.IsNullOrEmpty(type.Namespace))
                {
                    append(type.Namespace);
                    append(".");
                }

                FormatGenericType(type, append);
            }
            else if (type.BaseType == typeof(Array))
            {
                FormatArrayType(type, append);
            }
            else if (LanguageKeywords.TryGetValue(type, out string alias))
            {
                append(alias);
            }
            else
            {
                FormatClrName(type, append, withLeadingNamespace);
            }
        }

        /// <summary>
        /// Format type like the CLR does.
        /// </summary>
        /// <param name="type">The type for format</param>
        /// <param name="append">Function for appending the formatted type part</param>
        /// <param name="withLeadingNamespace">I true, use the full name, otherwise only the class name</param>
        private static void FormatClrName(Type type, Action<string> append, bool withLeadingNamespace)
        {
            if (withLeadingNamespace)
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
        /// <param name="type">The array type. Can have any dimension</param>
        /// <param name="append">Function for appending the formatted type part</param>
        private static void FormatArrayType(Type type, Action<string> append)
        {
            FormatType(type.GetElementType(), append);

            append("[");
            append(new string(',', type.GetArrayRank() - 1));
            append("]");
        }

        /// <summary>
        /// Format a bound or unbound generic type
        /// </summary>
        /// <param name="type">The generic type. Can be bound or unbound generic</param>
        /// <param name="append">Function for appending the formatted type part</param>
        private static void FormatGenericType(Type type, Action<string> append)
        {
            FormatDeclaringTypeNames(type, append);

            append(type.Name[..type.Name.LastIndexOf('`')]);
            append("<");

            FormatGenericArguments(type, append);

            append(">");
        }

        /// <summary>
        /// Format generic arguments of a type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="append">Function for appending the formatted type part</param>
        private static void FormatGenericArguments(Type type, Action<string> append)
        {
            Type[] types = type.GetGenericArguments();
            bool isUnboundType = type.ContainsGenericParameters;
            if (isUnboundType)
            {
                append(new string(',', types.Length - 1));
            }
            else
            {
                int numberOfGenericArguments = GetNumberOfGenericArguments(type);
                int firstGenericArgumentIndes = types.Length - numberOfGenericArguments;
                for (int i = firstGenericArgumentIndes; i < types.Length; i++)
                {
                    FormatType(types[i], append);
                    if (i < types.Length - 1)
                    {
                        append(", ");
                    }
                }
            }
        }

        /// <summary>
        /// Get number of generic arguments of a type.
        /// </summary>
        /// <remarks>
        /// For nested generic classes like class A&lt;T&gt; { class B&lt;T2&gt; { } },
        /// the inner class also hold the generic arguments of the parent classes,
        /// which we don't want for the formatting.
        /// </remarks>
        /// <param name="type">The generic type of which to get the generic arguments</param>
        /// <returns>The count of generic arguments which are associated only with <paramref name="type"/></returns>
        private static int GetNumberOfGenericArguments(Type type)
        {
            // the innermost nested class holds all generic arguments including the ones of the declaring types.
            // so we must check, which ones to write out.
            return int.Parse(type.Name[(type.Name.LastIndexOf('`') + 1)..], CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Format the declaring type names, i.e. the base classes, of a type.
        /// </summary>
        /// <param name="type">The child type, of which the declaring types to format</param>
        /// <param name="append">Function for appending the formatted type part</param>
        private static void FormatDeclaringTypeNames(Type type, Action<string> append)
        {
            foreach (Type declaringType in GetDeclaringTypes(type))
            {
                // for the declaring types we stick to the default, short notation like Dictionary`2.
                append(declaringType.Name);
                append("+");
            }
        }

        private static IReadOnlyList<Type> GetDeclaringTypes(Type type)
        {
            if (type.DeclaringType is null)
            {
                return [];
            }

            List<Type> declaringTypes = [];
            while (type.DeclaringType is Type declaringType)
            {
                declaringTypes.Insert(0, declaringType);
                type = declaringType;
            }

            return declaringTypes;
        }

        /// <summary>
        /// Lookup dictionary to use language keyword instead of type name
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
            { typeof(void), "void" }, // Yes, this is an odd one. Technically it's a type though.
        };
    }
}
