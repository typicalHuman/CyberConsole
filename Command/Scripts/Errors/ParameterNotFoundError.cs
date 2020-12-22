namespace Command.Errors
{
    public class ParameterNotFoundError: Error
    {
        private string ParameterStringValue { get; set; } = string.Empty;

        /// <summary>
        /// Initialize string value of parameter.
        /// </summary>
        /// <param name="paramStringValue">String value of parameter.</param>
        public ParameterNotFoundError(string paramStringValue = "")
        {
            ParameterStringValue = paramStringValue;
            Message = $"Parameter '{ParameterStringValue}' not found.";
        }
        public override ErrorTypes ErrorType { get; protected set; } = ErrorTypes.NOT_FOUND;
        public override string Message { get; protected set; } 
    }
}
