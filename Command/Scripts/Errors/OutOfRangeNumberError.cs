namespace Command.Errors
{
    public class OutOfRangeNumberError: Error
    {
        private int Start { get; set; }
        private int End { get; set; }
        public OutOfRangeNumberError(int start = 1, int end = 255)
        {
            Start = start;
            End = end;
            Message = $"Out of range number! Input number between [{Start}, {End}].";
        }
        public override ErrorTypes ErrorType { get; protected set; } = ErrorTypes.OUT_OF_RANGE_NUMBER;
        public override string Message { get; protected set; }
    }
}
