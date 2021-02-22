using Command.Errors;
using Command.Interfaces;

namespace Commands.StandardCommands.RemoveCommandCmnd.Attributes
{
    internal class AllAttribute: IAttrib
    {
        public void Action(object[] args = null)
        {
            int count = ProjectManager.GetModules().Count;
            while (count > 0)
                ProjectManager.RemoveModule(0);
            Message = "All modules are removed.";
        }

        public string Message { get; set; }

        public string Value { get; set; } = "-A";

        public bool Equals(string parameter)
        {
            return Value == parameter;
        }

        public Error Error { get; set; }
        public int Offset { get; set; }
        public int EndOffset { get; set; }

        public string Description { get; set; } = "attribute for deleting all modules;";
    }
}
