using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AwesomeAssertions.Common;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Equivalency.Specs;

public partial class SelectionRulesSpecs
{
    public class Excluding
    {
        [Fact]
        public void A_member_excluded_by_path_is_described_in_the_failure_message()
        {
            var subject = new { Name = "John", Age = 13 };
            var customer = new { Name = "Jack", Age = 37 };

            Action act = () => subject.Should().BeEquivalentTo(customer, options => options.Excluding(d => d.Age));

            act.Should().Throw<XunitException>().WithMessage("*Exclude*Age*");
        }

        [Fact]
        public void A_member_excluded_by_predicate_is_described_in_the_failure_message()
        {
            var subject = new { Name = "John", Age = 13 };
            var customer = new { Name = "Jack", Age = 37 };

            Action act = () => subject.Should().BeEquivalentTo(customer, options => options.Excluding(ctx => ctx.Path == "Age"));

            act.Should().Throw<XunitException>().WithMessage("*Exclude member when*Age*");
        }

        [Fact]
        public void A_member_excluded_by_name_is_described_in_the_failure_message()
        {
            var subject = new { Id = 1, Name = "John", Age = 13 };
            var customer = new { Id = 2, Name = "Jack", Age = 37 };

            Action act = () => subject.Should().BeEquivalentTo(customer, options => options.ExcludingMembersNamed("Name", "Age"));

            act.Should().Throw<XunitException>().WithMessage("*Exclude members named: Name, Age*");
        }

        [Fact]
        public void Excluding_member_by_name_should_succeed()
        {
            var subject = new { Id = 1, Name = "John", Age = 13 };
            var customer = new { Id = 1, Name = "Jack", Age = 13 };

            subject.Should().BeEquivalentTo(customer, options => options.ExcludingMembersNamed("Name"));
        }

        [Fact]
        public void Excluding_member_by_name_respects_casing()
        {
            var subject = new { Name = "John" };
            var customer = new { Name = "Jack" };

            Action act = () => subject.Should().BeEquivalentTo(customer, options => options.ExcludingMembersNamed("name"));

            act.Should().Throw<XunitException>()
                .Which.Message.Should().Match("Expected property subject.Name*- Exclude members named: name*");
        }

        [Fact]
        public void Excluding_member_by_name_should_pass_in_nested_types()
        {
            var subject = new { Id = 1, Nested = new { Name = "John", Age = 13 } };
            var customer = new { Id = 1, Nested = new { Age = 13 } };

            subject.Should().BeEquivalentTo(customer, options => options.ExcludingMembersNamed("Name"));
        }

        [Fact]
        public void Passing_a_null_parameter_as_member_names_should_throw()
        {
            var subject = new { Age = 10 };

            Action act = () => subject.Should().BeEquivalentTo(subject, options => options.ExcludingMembersNamed(null!));

            act.Should().Throw<ArgumentNullException>().WithParameterName("memberNames");
        }

        [Fact]
        public void Passing_an_empty_collection_parameter_as_member_names_should_throw()
        {
            var subject = new { Age = 10 };

            Action act = () => subject.Should().BeEquivalentTo(subject, options => options.ExcludingMembersNamed([]));

            act.Should().Throw<ArgumentException>()
                .WithParameterName("memberNames")
                .WithMessage("At least one member name must be specified*memberNames*");
        }

        [Fact]
        public void When_only_the_excluded_property_doesnt_match_it_should_not_throw()
        {
            var dto = new CustomerDto { Age = 36, Birthdate = new DateTime(1973, 9, 20), Name = "John" };
            var customer = new Customer { Age = 36, Birthdate = new DateTime(1973, 9, 20), Name = "Dennis" };

            dto.Should().BeEquivalentTo(customer, options => options
                .Excluding(d => d.Name)
                .Excluding(d => d.Id));
        }

        [Fact]
        public void When_only_the_excluded_property_doesnt_match_it_should_not_throw_if_root_is_a_collection()
        {
            var dto = new Customer { Age = 36, Birthdate = new DateTime(1973, 9, 20), Name = "John" };
            var customer = new Customer { Age = 36, Birthdate = new DateTime(1973, 9, 20), Name = "Dennis" };

            new[] { dto }.Should().BeEquivalentTo([customer], options => options.Excluding(d => d.Name).Excluding(d => d.Id));
        }

        [Fact]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void When_excluding_members_it_should_pass_if_only_the_excluded_members_are_different()
        {
            var class1 = new ClassWithSomeFieldsAndProperties
                { Field1 = "Lorem", Field2 = "ipsum", Field3 = "dolor", Property1 = "sit" };
            var class2 = new ClassWithSomeFieldsAndProperties
                { Field1 = "Lorem", Field2 = "ipsum" };

            class1.Should().BeEquivalentTo(class2,
                opts => opts.Excluding(o => o.Field3).Excluding(o => o.Property1));
        }

        [Fact]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void When_excluding_members_it_should_fail_if_any_non_excluded_members_are_different()
        {
            var class1 = new ClassWithSomeFieldsAndProperties
                { Field1 = "Lorem", Field2 = "ipsum", Field3 = "dolor", Property1 = "sit" };
            var class2 = new ClassWithSomeFieldsAndProperties
                { Field1 = "Lorem", Field2 = "ipsum" };

            Action act =
                () => class1.Should().BeEquivalentTo(class2, opts => opts.Excluding(o => o.Property1));

            act.Should().Throw<XunitException>().WithMessage("Expected*Field3*");
        }

        [Fact]
        public void When_excluding_member_names_it_should_pass_if_only_the_excluded_members_are_different()
        {
            var class1 = new ClassWithSomeFieldsAndProperties
                { Field1 = "First", Field2 = "Second", Field3 = "Third", Property1 = "FirstProperty" };
            var class2 = new ClassWithSomeFieldsAndProperties
                { Field1 = "First", Field2 = "Second" };

            class1.Should().BeEquivalentTo(class2,
                opts => opts.ExcludingMembersNamed("Field3", "Property1"));
        }

        [Fact]
        public void When_excluding_member_names_it_should_fail_if_any_non_excluded_members_are_different()
        {
            // Arrange
            var class1 = new ClassWithSomeFieldsAndProperties
                { Field1 = "First", Field2 = "Second", Field3 = "Third", Property1 = "FirstProperty" };
            var class2 = new ClassWithSomeFieldsAndProperties
                { Field1 = "First", Field2 = "Second" };

            Action act =
                () => class1.Should().BeEquivalentTo(class2, opts => opts.ExcludingMembersNamed("Property1"));

            act.Should().Throw<XunitException>().WithMessage("Expected*Field3*");
        }

        [Fact]
        public void When_all_shared_properties_match_it_should_not_throw()
        {
            var dto = new CustomerDto { Age = 36, Birthdate = new DateTime(1973, 9, 20), Name = "John" };
            var customer = new Customer { Id = 1, Age = 36, Birthdate = new DateTime(1973, 9, 20), Name = "John" };

            dto.Should().BeEquivalentTo(customer, options => options.ExcludingMissingMembers());
        }

        [Fact]
        public void When_a_deeply_nested_property_with_a_value_mismatch_is_excluded_it_should_not_throw()
        {
            var subject = new Root
            {
                Text = "Root",
                Level = new Level1 { Text = "Level1", Level = new Level2 { Text = "Mismatch" } }
            };
            var expected = new RootDto
            {
                Text = "Root",
                Level = new Level1Dto { Text = "Level1", Level = new Level2Dto { Text = "Level2" } }
            };

            subject.Should().BeEquivalentTo(expected, options => options.Excluding(r => r.Level.Level.Text));
        }

        [Fact]
        public void When_a_deeply_nested_property_with_a_value_mismatch_is_excluded_it_should_not_throw_if_root_is_a_collection()
        {
            var subject = new Root
            {
                Text = "Root",
                Level = new Level1 { Text = "Level1", Level = new Level2 { Text = "Mismatch" } }
            };
            var expected = new RootDto
            {
                Text = "Root",
                Level = new Level1Dto { Text = "Level1", Level = new Level2Dto { Text = "Level2" } }
            };

            new[] { subject }.Should().BeEquivalentTo([expected], options => options.Excluding(r => r.Level.Level.Text));
        }

        [Fact]
        public void When_a_property_with_a_value_mismatch_is_excluded_using_a_predicate_it_should_not_throw()
        {
            var subject = new Root
            {
                Text = "Root",
                Level = new Level1 { Text = "Level1", Level = new Level2 { Text = "Mismatch" } }
            };
            var expected = new RootDto
            {
                Text = "Root",
                Level = new Level1Dto { Text = "Level1", Level = new Level2Dto { Text = "Level2" } }
            };

            subject.Should().BeEquivalentTo(expected, config =>
                config.Excluding(ctx => ctx.Path == "Level.Level.Text"));
        }

        [Fact]
        public void When_members_are_excluded_by_the_access_modifier_of_the_getter_using_a_predicate_they_should_be_ignored()
        {
            var subject = new ClassWithAllAccessModifiersForMembers("public", "protected",
                "internal", "protected-internal", "private", "private-protected");
            var expected = new ClassWithAllAccessModifiersForMembers("public", "protected",
                "ignored-internal", "ignored-protected-internal", "private", "private-protected");

            subject.Should().BeEquivalentTo(expected, config => config
                .IncludingInternalFields()
                .Excluding(ctx =>
                    ctx.WhichGetterHas(CSharpAccessModifier.Internal) ||
                    ctx.WhichGetterHas(CSharpAccessModifier.ProtectedInternal)));
        }

        [Fact]
        public void When_members_are_excluded_by_the_access_modifier_of_the_setter_using_a_predicate_they_should_be_ignored()
        {
            var subject = new ClassWithAllAccessModifiersForMembers("public", "protected",
                "internal", "protected-internal", "private", "private-protected");
            var expected = new ClassWithAllAccessModifiersForMembers("public", "protected",
                "ignored-internal", "ignored-protected-internal", "ignored-private", "private-protected");

            subject.Should().BeEquivalentTo(expected, config => config
                .IncludingInternalFields()
                .Excluding(ctx =>
                    ctx.WhichSetterHas(CSharpAccessModifier.Internal) ||
                    ctx.WhichSetterHas(CSharpAccessModifier.ProtectedInternal) ||
                    ctx.WhichSetterHas(CSharpAccessModifier.Private)));
        }

        [Fact]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void When_excluding_properties_it_should_still_compare_fields()
        {
            var class1 = new ClassWithSomeFieldsAndProperties
            {
                Field1 = "Lorem", Field2 = "ipsum", Field3 = "dolor",
                Property1 = "sit", Property2 = "amet", Property3 = "consectetur"
            };
            var class2 = new ClassWithSomeFieldsAndProperties
            {
                Field1 = "Lorem", Field2 = "ipsum", Field3 = "color"
            };

            Action act = () => class1.Should().BeEquivalentTo(class2, opts => opts.ExcludingProperties());

            act.Should().Throw<XunitException>().WithMessage("*dolor*color*");
        }

        [Fact]
        public void When_excluding_fields_it_should_still_compare_properties()
        {
            var class1 = new ClassWithSomeFieldsAndProperties
            {
                Field1 = "Lorem", Field2 = "ipsum", Field3 = "dolor",
                Property1 = "sit", Property2 = "amet", Property3 = "consectetur"
            };
            var class2 = new ClassWithSomeFieldsAndProperties
            {
                Property1 = "sit", Property2 = "amet", Property3 = "different"
            };

            Action act = () => class1.Should().BeEquivalentTo(class2, opts => opts.ExcludingFields());

            act.Should().Throw<XunitException>().WithMessage("*Property3*consectetur*");
        }

        [Fact]
        public void When_excluding_properties_via_non_array_indexers_it_should_exclude_the_specified_paths()
        {
            var subject = new
            {
                List = new[] { new { Foo = 1, Bar = 2 }, new { Foo = 3, Bar = 4 } }.ToList(),
                Dictionary = new Dictionary<string, ClassWithOnlyAProperty>
                {
                    ["Foo"] = new() { Value = 1 },
                    ["Bar"] = new() { Value = 2 }
                }
            };
            var expected = new
            {
                List = new[] { new { Foo = 1, Bar = 2 }, new { Foo = 2, Bar = 4 } }.ToList(),
                Dictionary = new Dictionary<string, ClassWithOnlyAProperty>
                {
                    ["Foo"] = new() { Value = 1 },
                    ["Bar"] = new() { Value = 3 }
                }
            };

            subject.Should().BeEquivalentTo(expected,
                options => options
                    .Excluding(x => x.List[1].Foo)
                    .Excluding(x => x.Dictionary["Bar"].Value));
        }

        [Fact]
        public void
            When_excluding_properties_via_non_array_indexers_it_should_exclude_the_specified_paths_if_root_is_a_collection()
        {
            var subject = new
            {
                List = new[] { new { Foo = 1, Bar = 2 }, new { Foo = 3, Bar = 4 } }.ToList(),
                Dictionary = new Dictionary<string, ClassWithOnlyAProperty>
                {
                    ["Foo"] = new() { Value = 1 },
                    ["Bar"] = new() { Value = 2 }
                }
            };
            var expected = new
            {
                List = new[] { new { Foo = 1, Bar = 2 }, new { Foo = 2, Bar = 4 } }.ToList(),
                Dictionary = new Dictionary<string, ClassWithOnlyAProperty>
                {
                    ["Foo"] = new() { Value = 1 },
                    ["Bar"] = new() { Value = 3 }
                }
            };

            new[] { subject }.Should().BeEquivalentTo([expected],
                options => options
                    .Excluding(x => x.List[1].Foo)
                    .Excluding(x => x.Dictionary["Bar"].Value));
        }

        [Fact]
        public void When_excluding_properties_via_non_array_indexers_it_should_not_exclude_paths_with_different_indexes()
        {
            var subject = new
            {
                List = new[] { new { Foo = 1, Bar = 2 }, new { Foo = 3, Bar = 4 } }.ToList(),
                Dictionary = new Dictionary<string, ClassWithOnlyAProperty>
                {
                    ["Foo"] = new() { Value = 1 },
                    ["Bar"] = new() { Value = 2 }
                }
            };
            var expected = new
            {
                List = new[] { new { Foo = 5, Bar = 2 }, new { Foo = 2, Bar = 4 } }.ToList(),
                Dictionary = new Dictionary<string, ClassWithOnlyAProperty>
                {
                    ["Foo"] = new() { Value = 6 },
                    ["Bar"] = new() { Value = 3 }
                }
            };

            Action act = () =>
                subject.Should().BeEquivalentTo(expected,
                    options => options
                        .Excluding(x => x.List[1].Foo)
                        .Excluding(x => x.Dictionary["Bar"].Value));

            act.Should().Throw<XunitException>();
        }

        [Fact]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void
            When_configured_for_runtime_typing_and_properties_are_excluded_the_runtime_type_should_be_used_and_properties_should_be_ignored()
        {
            object class1 = new ClassWithSomeFieldsAndProperties
            {
                Field1 = "Lorem", Field2 = "ipsum", Field3 = "dolor",
                Property1 = "sit", Property2 = "amet", Property3 = "consectetur"
            };
            object class2 = new ClassWithSomeFieldsAndProperties
            {
                Field1 = "Lorem", Field2 = "ipsum", Field3 = "dolor"
            };

            class1.Should().BeEquivalentTo(class2, opts => opts.ExcludingProperties().PreferringRuntimeMemberTypes());
        }

        [Fact]
        public void When_excluding_virtual_or_abstract_property_exclusion_works_properly()
        {
            var obj1 = new Derived { DerivedProperty1 = "Something", DerivedProperty2 = "A" };
            var obj2 = new Derived { DerivedProperty1 = "Something", DerivedProperty2 = "B" };

            obj1.Should().BeEquivalentTo(obj2, opt => opt
                .Excluding(o => o.AbstractProperty)
                .Excluding(o => o.VirtualProperty)
                .Excluding(o => o.DerivedProperty2));
        }

        [Fact]
        public void Abstract_properties_cannot_be_excluded()
        {
            var obj1 = new Derived { DerivedProperty1 = "Something", DerivedProperty2 = "A" };
            var obj2 = new Derived { DerivedProperty1 = "Something", DerivedProperty2 = "B" };

            Action act = () => obj1.Should().BeEquivalentTo(obj2, opt => opt
                .Excluding(o => o.AbstractProperty + "B"));

            act.Should().Throw<ArgumentException>()
                .WithMessage("*(o.AbstractProperty + \"B\")*cannot be used to select a member*");
        }

#if NETCOREAPP3_0_OR_GREATER
        [Fact]
        public void Can_exclude_a_default_interface_property_using_an_expression()
        {
            IHaveDefaultProperty subject = new ClassReceivedDefaultInterfaceProperty { NormalProperty = "Value" };
            IHaveDefaultProperty expectation = new ClassReceivedDefaultInterfaceProperty { NormalProperty = "Another Value" };

            var act = () => subject.Should().BeEquivalentTo(expectation,
                x => x.Excluding(p => p.DefaultProperty));

            act.Should().Throw<XunitException>().Which.Message.Should().NotContain("subject.DefaultProperty");
        }

        [Fact]
        public void Can_exclude_a_default_interface_property_using_a_name()
        {
            IHaveDefaultProperty subject = new ClassReceivedDefaultInterfaceProperty { NormalProperty = "Value" };
            IHaveDefaultProperty expectation = new ClassReceivedDefaultInterfaceProperty { NormalProperty = "Another Value" };

            var act = () => subject.Should().BeEquivalentTo(expectation,
                x => x.Excluding(info => info.Name.Contains("DefaultProperty")));

            act.Should().Throw<XunitException>().Which.Message.Should().NotContain("subject.DefaultProperty");
        }

        private class ClassReceivedDefaultInterfaceProperty : IHaveDefaultProperty
        {
            public string NormalProperty { get; set; }
        }

        private interface IHaveDefaultProperty
        {
            string NormalProperty { get; set; }

            int DefaultProperty => NormalProperty.Length;
        }
#endif

        [Fact]
        public void An_anonymous_object_with_multiple_fields_excludes_correctly()
        {
            var subject = new { FirstName = "John", MiddleName = "X", LastName = "Doe", Age = 34 };
            var expectation = new { FirstName = "John", MiddleName = "W.", LastName = "Smith", Age = 29 };

            subject.Should().BeEquivalentTo(expectation, opts => opts
                .Excluding(p => new { p.MiddleName, p.LastName, p.Age }));
        }

        [Fact]
        public void An_empty_anonymous_object_excludes_nothing()
        {
            var subject = new { FirstName = "John", MiddleName = "X", LastName = "Doe", Age = 34 };
            var expectation = new { FirstName = "John", MiddleName = "W.", LastName = "Smith", Age = 29 };

            Action act = () => subject.Should().BeEquivalentTo(expectation, opts => opts
                .Excluding(p => new { }));

            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void An_anonymous_object_can_exclude_collections()
        {
            var subject = new { Names = new[] { "John", "X.", "Doe" }, Age = 34 };
            var expectation = new { Names = new[] { "John", "W.", "Smith" }, Age = 34 };

            subject.Should().BeEquivalentTo(expectation, opts => opts
                .Excluding(p => new { p.Names }));
        }

        [Fact]
        public void An_anonymous_object_can_exclude_nested_objects()
        {
            var subject = new { Names = new { FirstName = "John", MiddleName = "X", LastName = "Doe" }, Age = 34 };
            var expectation = new { Names = new { FirstName = "John", MiddleName = "W.", LastName = "Smith" }, Age = 34 };

            subject.Should().BeEquivalentTo(expectation, opts => opts
                .Excluding(p => new { p.Names.MiddleName, p.Names.LastName }));
        }

        [Fact]
        public void An_anonymous_object_can_exclude_nested_objects_inside_collections()
        {
            var subject = new
            {
                Names = new { FirstName = "John", MiddleName = "X", LastName = "Doe" },
                Pets = new[]
                {
                    new { Name = "Dog", Age = 1, Color = "Black" },
                    new { Name = "Cat", Age = 1, Color = "Black" },
                    new { Name = "Bird", Age = 1, Color = "Black" }
                }
            };
            var expectation = new
            {
                Names = new { FirstName = "John", MiddleName = "W.", LastName = "Smith" },
                Pets = new[]
                {
                    new { Name = "Dog", Age = 1, Color = "Black" },
                    new { Name = "Dog", Age = 2, Color = "Gray" },
                    new { Name = "Bird", Age = 3, Color = "Black" }
                }
            };

            Action act = () => subject.Should().BeEquivalentTo(expectation, opts => opts
                .Excluding(p => new { p.Names.MiddleName, p.Names.LastName })
                .For(p => p.Pets)
                .Exclude(p => new { p.Age, p.Name }));

            act.Should().Throw<XunitException>().Which.Message.Should()
                .NotMatch("*Pets[1].Age*").And
                .NotMatch("*Pets[1].Name*").And
                .Match("*Pets[1].Color*");
        }

        [Fact]
        public void An_anonymous_object_can_exclude_nested_objects_inside_nested_collections()
        {
            var subject = new
            {
                Names = new { FirstName = "John", MiddleName = "W.", LastName = "Smith" },
                Pets = new[]
                {
                    new
                    {
                        Name = "Dog",
                        Fleas = new[] { new { Name = "Flea 1", Age = 1 }, new { Name = "Flea 2", Age = 2 } }
                    },
                    new
                    {
                        Name = "Dog",
                        Fleas = new[] { new { Name = "Flea 10", Age = 1 }, new { Name = "Flea 21", Age = 3 } }
                    },
                    new
                    {
                        Name = "Dog",
                        Fleas = new[] { new { Name = "Flea 1", Age = 1 }, new { Name = "Flea 2", Age = 2 } }
                    }
                }
            };
            var expectation = new
            {
                Names = new { FirstName = "John", MiddleName = "W.", LastName = "Smith" },
                Pets = new[]
                {
                    new
                    {
                        Name = "Dog",
                        Fleas = new[] { new { Name = "Flea 1", Age = 1 }, new { Name = "Flea 2", Age = 2 } }
                    },
                    new
                    {
                        Name = "Dog",
                        Fleas = new[] { new { Name = "Flea 1", Age = 1 }, new { Name = "Flea 2", Age = 1 } }
                    },
                    new
                    {
                        Name = "Bird",
                        Fleas = new[] { new { Name = "Flea 1", Age = 1 }, new { Name = "Flea 2", Age = 2 } }
                    }
                }
            };

            Action act = () => subject.Should().BeEquivalentTo(expectation, opts => opts
                .Excluding(p => new { p.Names.MiddleName, p.Names.LastName })
                .For(person => person.Pets)
                .For(pet => pet.Fleas)
                .Exclude(flea => new { flea.Name, flea.Age }));

            act.Should().Throw<XunitException>().Which.Message.Should()
                .NotMatch("*Pets[*].Fleas[*].Age*").And
                .NotMatch("*Pets[*].Fleas[*].Name*").And
                .Match("*- Exclude*Pets[]Fleas[]Age*").And
                .Match("*- Exclude*Pets[]Fleas[]Name*");
        }

        [Fact]
        public void An_empty_anonymous_object_excludes_nothing_inside_collections()
        {
            var subject = new
            {
                Names = new { FirstName = "John", MiddleName = "X", LastName = "Doe" },
                Pets = new[]
                {
                    new { Name = "Dog", Age = 1 },
                    new { Name = "Cat", Age = 1 },
                    new { Name = "Bird", Age = 1 }
                }
            };
            var expectation = new
            {
                Names = new { FirstName = "John", MiddleName = "W.", LastName = "Smith" },
                Pets = new[]
                {
                    new { Name = "Dog", Age = 1 },
                    new { Name = "Dog", Age = 2 },
                    new { Name = "Bird", Age = 1 }
                }
            };

            Action act = () => subject.Should().BeEquivalentTo(expectation, opts => opts
                .Excluding(p => new { p.Names.MiddleName, p.Names.LastName })
                .For(p => p.Pets)
                .Exclude(p => new { }));

            act.Should().Throw<XunitException>()
                .WithMessage("*Pets[1].Name*Pets[1].Age*");
        }
    }
}
