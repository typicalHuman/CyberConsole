namespace Command.Errors
{
    public class NullArgumentError:Error
    {
        public NullArgumentError(string argumentName = "")
        {
            Message = $"Argument {argumentName} was null!";
        }
        public override ErrorTypes ErrorType { get; protected set; } = ErrorTypes.NULL_VALUE;
        public override string Message { get; protected set; }
    }
}
