namespace Command.Interfaces
{
    /// <summary>
    /// Interface for help message displaying.
    /// </summary>
    public interface IDescription
    {
        /// <summary>
        /// Command (or attribute) description.
        /// </summary>
        string Description { get; }
    }
}
