using Command.Interfaces;
using Command.StandardParameters.ParameterParsers;

namespace Command.StandardParameters
{
    public class StringParameter: Parameter<string>
    {
        protected override IParameterParser<string> Parser { get; set; } = new StringParser();


        public static explicit operator StringParameter(string text)
        {
            return new StringParameter().GetParameter(text) as StringParameter;
        }

        public override string Description { get; protected set; } = "parameter for parsing quote expressions;";
    }
}
