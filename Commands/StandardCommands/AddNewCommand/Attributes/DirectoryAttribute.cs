using Command.Errors;
using Command.Interfaces;

namespace Commands.StandardCommands.AddNewCommand.Attributes
{
    internal class DirectoryAttribute: IAttrib
    {
        public void Action(object[] args = null)
        {

        }

        public string Value => "-d";
        public bool Equals(string parameter)
        {
            return Value.Equals(parameter);
        }

        public string Message { get; set; }

        public Error Error { get; set; }
        public int Offset { get; set; }
        public int EndOffset { get; set; }

        public virtual string Description { get; protected set; } = "'attribute for adding all files in the directory (and in the subdirectories);";
    }
}
