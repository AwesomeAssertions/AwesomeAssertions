using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using FluentAssertions.Execution;
using FluentAssertions.Types;
using Xunit;

namespace FluentAssertions.Specs;

public sealed class AssertionsApiSpecs
{
    [Fact]
    public void Assertions_types_have_property_names_by_convention()
    {
        static bool IsAssertionsClass(Type type)
        {
            if (type.IsDefined(typeof(CompilerGeneratedAttribute)))
            {
                return false;
            }

            string name = type.Name;
            int lastIndex = type.Name.LastIndexOf('`');
            if (lastIndex >= 0)
            {
                name = name[..lastIndex];
            }

            return name.EndsWith("Assertions", StringComparison.Ordinal);
        }

        Assembly awesomeAssertionsAssembly = typeof(AssertionScope).Assembly;
        TypeSelector assertionsClasses = awesomeAssertionsAssembly.Types()
            .ThatAreClasses()
            .ThatAreNotAbstract()
            .ThatAreNotStatic()
            .ThatSatisfy(IsAssertionsClass);

        assertionsClasses.AsEnumerable().Should().AllSatisfy(
            type => type.Should().HaveProperty<AssertionChain>("CurrentAssertionChain"));
    }
}
