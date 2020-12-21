using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Command.Interfaces;
using Command.Parameters.ParameterParsers;

namespace Command.Parameters
{
    public class BracketParameter: Parameter<string>
    {
        protected override IParameterParser<string> Parser { get; set; } = new BracketParser();
    }
}
