namespace Command.Errors
{
    class NoneError: Error
    {
        public override ErrorTypes ErrorType { get; protected set; } = ErrorTypes.NONE;
        public override string Message { get; protected set; }
    }
}
