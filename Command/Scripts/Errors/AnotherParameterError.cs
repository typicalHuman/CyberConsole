namespace Command.Errors
{
    public class AnotherParameterError : Error
    {
        public override ErrorTypes ErrorType { get; protected set; } = ErrorTypes.ANOTHER_PARAMETER;
        public override string Message { get; protected set; } = "Syntax error!";
    }
}
