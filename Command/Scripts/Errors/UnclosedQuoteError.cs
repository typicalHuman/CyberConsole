namespace Command.Errors
{
    public class UnclosedQuoteError: Error
    {
        public override ErrorTypes ErrorType { get; protected set; } = ErrorTypes.UNCLOSED_QUOTE;
        public override string Message { get; protected set; } = "Close the quotes!";
    }
}
