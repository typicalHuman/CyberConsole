using Command.Errors;

namespace Command.Interfaces
{
    /// <summary>
    /// Random data storage interface.
    /// </summary>
    public interface IParameter
    {
        /// <summary>
        /// String value of parameter.
        /// </summary>
        string Value { get; }

        /// <summary>
        /// Spellings compare logic.
        /// </summary>
        bool Equals(string parameter);

        /// <summary>
        /// Error value.
        /// </summary>
        Error Error { get; set; }

        /// <summary>
        /// Offset in command line text.
        /// </summary>
        int Offset { get; set; }

        /// <summary>
        /// EndOffset in command line text.
        /// </summary>
        int EndOffset { get; set; }
    }
}
