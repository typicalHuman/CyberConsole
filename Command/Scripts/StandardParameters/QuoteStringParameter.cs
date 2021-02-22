using Command.Interfaces;
using Command.StandardParameters.ParameterParsers;

namespace Command.StandardParameters
{
    public class QuoteStringParameter : Parameter<string>
    {
        protected override IParameterParser<string> Parser { get; set; } = new StringParser();


        public static explicit operator QuoteStringParameter(string text)
        {
            return new QuoteStringParameter().GetParameter(text) as QuoteStringParameter;
        }

        public override string Description { get; protected set; } = "parameter for parsing single quote expressions;";
    }
}
