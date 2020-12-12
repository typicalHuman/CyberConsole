namespace Command.Errors
{
    class LetterToDigitError: Error
    {
        public override ErrorTypes ErrorType { get; protected set; } = ErrorTypes.WRONG_INPUT;
        public override string Message { get; protected set; } = "Cannot convert letter to a number.";
    }
}
