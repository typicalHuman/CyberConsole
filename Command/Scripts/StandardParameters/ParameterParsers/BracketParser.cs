using Command.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Command.StandardParameters.ParameterParsers
{
    class BracketParser: ParameterParser<string>
    {
        private const string REGEX_PATTERN = @"\(([^)]*)\)";

        public override string Parse(string input, out Error error)
        {
            string[] matches = GetMatches(input, REGEX_PATTERN);
            if (matches.Length == 0 || input[0] != '(' || input[input.Length - 1] != ')')
            {
                if (input.Contains('(') || input.Contains(')'))//check is input contains bracket
                    error = new UnclosedQuoteError();
                else
                    error = new AnotherParameterError();
                return input;
            }
            string result = String.Join("", matches.ToArray());
            error = new NoneError();
            return result.Remove(0, 1)                 //removing
                         .Remove(result.Length - 2, 1);//brackets
        }
    }
}
