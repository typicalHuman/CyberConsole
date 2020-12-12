using Command.Errors;
using Command.Interfaces;
using System.Linq;
using System.Text.RegularExpressions;

namespace Command.Parameters.ParameterParsers
{
    /// <summary>
    /// Abstract class for creating parameters parser.
    /// </summary>
    /// <typeparam name="T">Type of parameters to parse values.</typeparam>
    public abstract class ParameterParser<T> : IParameterParser<T>
    {
        public abstract T Parse(string input, out Error error);

        /// <summary>
        /// Get matches by <paramref name="regexPattern"/> in <paramref name="input"/>.
        /// </summary>
        /// <param name="input">Input string for matches searching.</param>
        /// <param name="regexPattern">Pattern for searching.</param>
        /// <returns>Array of matches.</returns>
        protected string[] GetMatches(string input, string regexPattern)
        {
            string[] matches = (from Match match in Regex.Matches(input, regexPattern)
                                select match.ToString()).ToArray();
            return matches;
        }
    }
}
