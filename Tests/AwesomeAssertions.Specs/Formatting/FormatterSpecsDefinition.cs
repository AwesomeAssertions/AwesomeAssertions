using Xunit;

namespace AwesomeAssertions.Specs.Formatting;

// Due to the tests that (temporarily) modify the active formatters collection.
[CollectionDefinition("FormatterSpecs", DisableParallelization = true)]
public class FormatterSpecsDefinition;
