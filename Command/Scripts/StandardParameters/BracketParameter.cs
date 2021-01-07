using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Command.Interfaces;
using Command.StandardParameters.ParameterParsers;

namespace Command.StandardParameters
{
    public class BracketParameter: Parameter<string>
    {
        protected override IParameterParser<string> Parser { get; set; } = new BracketParser();
    }
}
