namespace Command.Errors
{
    public class WrongParametersCountError: Error
    {
        public WrongParametersCountError(string additionalMessage) : base(additionalMessage) { }
        public override ErrorTypes ErrorType { get; protected set; } = ErrorTypes.WRONG_PARAMETERS_COUNT;
        public override string Message { get; protected set; } = "Wrong parameters count.";
    }
}
