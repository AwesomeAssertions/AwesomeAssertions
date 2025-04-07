using Xunit;

namespace AwesomeAssertions.Specs;

// Try to stabilize UIFact tests
[CollectionDefinition("UIFacts", DisableParallelization = true)]
public class UIFactsDefinition;
