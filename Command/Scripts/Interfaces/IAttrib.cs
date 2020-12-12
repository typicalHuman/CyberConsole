namespace Command.Interfaces
{
    /// <summary>
    /// Attribute abstraction.
    /// </summary>
    public interface IAttrib: IParameter
    {
        /// <summary>
        /// Attrubute action (you can just change logic of behavior and do nothing in action method).
        /// </summary>
        void Action(object input = null);
    }
}
