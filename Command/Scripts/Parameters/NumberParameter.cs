using Command.Interfaces;
using Command.Parameters.ParameterParsers;

namespace Command.Parameters
{
    public class NumberParameter : Parameter<short>
    {
        protected override IParameterParser<short> Parser { get; set; } = new NumberParser();


        public static explicit operator NumberParameter(short number)
        {
            return new NumberParameter().GetParameter(number.ToString()) as NumberParameter;
        }
    }
}
