namespace Command.Errors
{
    public class SyntaxError: Error
    {
        public override ErrorTypes ErrorType { get; protected set; } = ErrorTypes.SYNTAX_ERROR;

        public override string Message { get; protected set; } = "Syntax error!";
    }
}
