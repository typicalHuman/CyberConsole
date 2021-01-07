using Command.Errors;
using System;
using System.Linq;

namespace Command.StandardParameters.ParameterParsers
{
    /// <summary>
    /// Class for parsing strings.
    /// </summary>
    public class StringParser : ParameterParser<string>
    {
        #region Constants

        /// <summary>
        /// Pattern for defining strings in quotes.
        /// </summary>
        private const string REGEX_PATTERN = ".*?(\\\".*\\\").*?";

        #endregion

        public override string Parse(string input, out Error error)
        {
            string[] matches = GetMatches(input, REGEX_PATTERN);
            if (matches.Length == 0)
            {
                if (input.Contains('"'))//check is input contains quote
                    error = new UnclosedQuoteError();
                else
                    error = new AnotherParameterError();
                return input;
            }
            string result = String.Join("", matches.ToArray());
            error = new NoneError();
            return result.Remove(0, 1)                  //removing
                         .Remove(result.Length - 2 , 1);//quotes
        }
    }
}
