using Command.Interfaces;
using Command.Parameters.ParameterParsers;

namespace Command.Parameters
{
    public class NumberParameter : Parameter<byte>
    {
        protected override IParameterParser<byte> Parser { get; set; } = new NumberParser();


        public static explicit operator NumberParameter(byte number)
        {
            return new NumberParameter().GetParameter(number.ToString()) as NumberParameter;
        }
    }
}
