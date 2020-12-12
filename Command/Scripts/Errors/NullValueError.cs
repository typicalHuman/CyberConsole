namespace Command.Errors
{
    public class NullValueError : Error
    {
        public override ErrorTypes ErrorType { get; protected set; } = ErrorTypes.NULL_VALUE;
        public override string Message { get; protected set; } = "Value must be defined.";
    }
}
