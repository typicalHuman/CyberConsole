using Command;
using Command.Interfaces;
using Command.StandardParameters;

namespace Commands.StandardCommands
{
    class RemoveCommand : ConsoleCommand
    {
        public override IAttrib[] StandardAttributes { get; protected set; }
        public override IAttrib[] CurrentAttributes { get; set; }
        public override IParameter[] StandardParameters { get; protected set; } = new IParameter[]
        {
            new NumberParameter()
        };
        public override IParameter[] Parameters { get; set; }
        public override string Spelling { get; protected set; } = "rm_cmnd";
        public override string Description { get; protected set; } = "remove command by index;";

        public override void Action(string commandLineText, params object[] args)
        {
            SetParameters<NumberParameter, short>(commandLineText);
            Message = ProjectManager.RemoveModule(0);
        }
    }
}
