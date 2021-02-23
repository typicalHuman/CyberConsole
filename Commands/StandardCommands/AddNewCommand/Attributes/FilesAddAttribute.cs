using Command.Errors;
using Command.Interfaces;
namespace Commands.StandardCommands.AddNewCommand.Attributes
{

    internal abstract class FileAddAttribute : IAttrib
    {
        public virtual void Action(object[] args = null)
        {
            Action(args, null);
        }

        public abstract void Action(object[] args = null, string moduleName = null);

        public abstract string Value { get; protected set; }

        public bool Equals(string parameter)
        {
            return Value.Equals(parameter);
        }

        public string Message { get; protected set; }

        public Error Error { get; set; }
        public int Offset { get; set; }
        public int EndOffset { get; set; }

        public abstract string Description { get; protected set; }
    }
}
