using Command.Errors;

namespace Commands.StandardCommands.AddNewCommand.Attributes
{
    internal class DirectoryAttribute: FileAddAttribute
    {
        public override void Action(object[] args = null, string moduleName = null)
        {
            if (args == null || args.Length == 0)
                Error = new NullValueError();
            else
                Message = ProjectManager.AddDirectories(moduleName, args as string[]);
        }

        public override string Value { get; protected set; } = "-d";

        public override string Description { get; protected set; } = "'attribute for adding all files in the directories (and in the subdirectories);";
    }
}
