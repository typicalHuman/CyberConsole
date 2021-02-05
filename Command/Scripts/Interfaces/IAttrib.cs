namespace Command.Interfaces
{
    /// <summary>
    /// Attribute abstraction.
    /// </summary>
    public interface IAttrib: IParameter, IDescription
    {
        /// <summary>
        /// Attrubute action (you can just change logic of behavior and do nothing in action method).
        /// </summary>
        void Action(object[] args = null);

        /// <summary>
        /// For transfering information about success of <see cref="Action(object[])"/>.
        /// </summary>
        string Message { get; }
    }
}
