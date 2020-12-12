using Command.Interfaces;
using Command.Parameters.ParameterParsers;

namespace Command.Parameters
{
    public class StringParameter: Parameter<string>
    {
        protected override IParameterParser<string> Parser { get; set; } = new StringParser();


        public static explicit operator StringParameter(string text)
        {
            return new StringParameter().GetParameter(text) as StringParameter;
        }
    }
}
