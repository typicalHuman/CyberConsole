namespace Command.Interfaces
{
    public interface IOffset
    {
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
