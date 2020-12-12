namespace Command.Interfaces
{
    /// <summary>
    /// Interface to define command parser logic.
    /// </summary>
    public interface IParser
    {
        /// <summary>
        /// Is command spelling equals console line command.
        /// </summary>
        bool IsCommandLexic(string commandSpelling, string commandLineText);
        /// <summary>
        /// Is attribute spellings equals console line attributes.
        /// </summary>
        bool IsAttributesLexic(IAttrib[] standardAttributes, string commandLineText);
        /// <summary>
        /// Get command attributes.
        /// </summary>
        IAttrib[] GetAttributes(ICommand command, string commandLineText);
    }
}
