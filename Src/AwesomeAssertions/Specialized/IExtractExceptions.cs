using System;
using System.Collections.Generic;

namespace AwesomeAssertions.Specialized;

public interface IExtractExceptions
{
    IEnumerable<T> OfType<T>(Exception actualException)
        where T : Exception;
}
