﻿namespace FluentAssertions.Assertions
{
    /// <summary>
    /// Defines the way <see cref="ExceptionAssertions{TException}.WithMessage(string)"/> compares the expected exception 
    /// message with the actual one.
    /// </summary>
    public enum ComparisonMode
    {
        /// <summary>
        /// The message must match exactly, including the casing of the characters.
        /// </summary>
        Exact,

        /// <summary>
        /// The message must contain the expected message.
        /// </summary>
        Substring,

        /// <summary>
        /// The message must match a wildcard pattern consisting of ordinary characters as well as * and ?.
        /// </summary>
        Wildcard
    }
}