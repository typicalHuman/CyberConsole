﻿using Command.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Command.Parameters.ParameterParsers
{
    class BracketParser: ParameterParser<string>
    {
        private const string REGEX_PATTERN = @"\(([^)]*)\)";

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
                         .Remove(result.Length - 2, 1);//quotes
        }
    }
}
