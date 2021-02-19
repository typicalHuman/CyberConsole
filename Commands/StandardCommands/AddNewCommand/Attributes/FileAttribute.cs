using Command.Errors;
using Command.Interfaces;

namespace Commands.StandardCommands.AddNewCommand.Attributes
{
    internal class FileAttribute: IAttrib
    {
        public void Action(object[] args = null)
        {
            if (args == null || args.Length == 0)
                Error = new NullValueError();
            else
                Message = ProjectManager.AddFiles(args as string[]);
        }

        public string Value => "-f";
        public bool Equals(string parameter)
        {
            return Value.Equals(parameter);
        }

        public string Message { get; private set; }

        public Error Error { get; set; }
        public int Offset { get; set; }
        public int EndOffset { get; set; }

        public virtual string Description { get; protected set; } = "attribute for saving one or more files;";
    }
}
