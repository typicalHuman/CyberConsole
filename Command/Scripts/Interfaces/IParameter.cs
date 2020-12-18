using Command.Errors;

namespace Command.Interfaces
{
    /// <summary>
    /// Random data storage interface.
    /// </summary>
    public interface IParameter: IOffset
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
    }
}
