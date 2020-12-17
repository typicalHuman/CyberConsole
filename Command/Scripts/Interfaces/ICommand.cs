namespace Command.Interfaces
{
    /// <summary>
    /// Command abstraction.
    /// </summary>
    public interface ICommand
    {
        string Spelling { get; }
        /// <summary>
        /// Command action.
        /// </summary>
        void Action(string commandLineText, params object[] args);
        /// <summary>
        /// Get statement line.
        /// </summary>
        string PrintInfo();
        /// <summary>
        /// Avaliable attributes for this command.
        /// </summary>
        IAttrib[] StandardAttributes { get; }
        /// <summary>
        /// Attributes that have been specified.
        /// </summary>
        IAttrib[] CurrentAttributes { get; set; }
        /// <summary>
        /// Avaliable parameters for this command. 
        /// </summary>
        IParameter[] StandardParameters { get; }
        /// <summary>
        /// Array of command parameters.
        /// </summary>
        IParameter[] Parameters { get; set; }
        bool IsCommandLexic(string commandLineText);

    }
}
