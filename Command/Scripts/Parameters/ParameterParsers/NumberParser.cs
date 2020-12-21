using Command.Errors;
using System.Text.RegularExpressions;

namespace Command.Parameters.ParameterParsers
{
    /// <summary>
    /// Class for parsing numbers.
    /// </summary>
    public class NumberParser : ParameterParser<short>
    {
        #region Constants

        /// <summary>
        /// Pattern for excluding out of range exception and negative iterations count.
        /// </summary>
        private const string REGEX_PATTERN = @"^\d{1,32767}$";

        /// <summary>
        /// Pattern for defining strings which are consisted of only letters.
        /// </summary>
        private const string REGEX_LETTERS_PATTERN = @"^[a-zA-Z]+$";

        #endregion

        public override short Parse(string input, out Error error)
        {
            string[] matches = GetMatches(input, REGEX_PATTERN);
            if(matches.Length == 0)
            {
                if (Regex.IsMatch(input, REGEX_LETTERS_PATTERN))
                    error = new LetterToDigitError();
                else
                    error = new AnotherParameterError();
                return default(short);
            }
            if(!short.TryParse(matches[0], out short result) || short.Parse(matches[0]) < 1)
            {
                error = new OutOfRangeNumberError();
                return default(short);
            }
            error = new NoneError();
            return short.Parse(matches[0]);
        }
    }
}
