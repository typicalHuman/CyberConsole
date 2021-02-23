using Command.Errors;

namespace Commands.StandardCommands.AddNewCommand.Attributes
{
    internal class FileAttribute: FileAddAttribute
    {
        public override void Action(object[] args = null, string moduleName = null)
        {
            if (args == null || args.Length == 0)
                Error = new NullValueError();
            else
                Message = ProjectManager.AddFiles(moduleName, args as string[]);
        }

        public override string Description { get; protected set; } = "attribute for saving one or more files;";

        public override string Value { get; protected set; } = "-f";
    }
}
