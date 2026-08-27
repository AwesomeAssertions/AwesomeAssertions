using AwesomeAssertions.Common;
using Xunit;

namespace AwesomeAssertions.Specs.Common;

public class MemberPathSpecs
{
    [Fact]
    public void The_hash_code_of_path_segment_comparer_shall_use_the_string_hash_code()
    {
        var sut = new MemberPathSegmentEqualityComparer();

        sut.GetHashCode("Test").Should().Be("Test".GetHashCode());
    }
}
