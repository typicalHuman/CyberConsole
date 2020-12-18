using Command.Interfaces;

namespace Command.Errors
{
    /// <summary>
    /// Class to define console errors.
    /// </summary>
    public abstract class Error: IOffset
    {
        /// <summary>
        /// Abstract error type.
        /// </summary>
        public abstract ErrorTypes ErrorType { get; protected set; }

        /// <summary>
        /// Message to show when the current error will appear. 
        /// </summary>
        public abstract string Message { get; protected set; } 


        public static explicit operator Error(ErrorTypes type)
        {
            if (type == ErrorTypes.SYNTAX_ERROR)
                return new SyntaxError();
            return new NoneError();
        }

        public int Offset { get; set; }
        public int EndOffset { get; set; }
    }
}
