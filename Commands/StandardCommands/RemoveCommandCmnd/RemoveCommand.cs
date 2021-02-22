using Command;
using Command.Interfaces;
using Command.Parsers;
using System.Linq;
using Command.StandardParameters;
using Commands.StandardCommands.RemoveCommandCmnd.Attributes;

namespace Commands.StandardCommands
{
    class RemoveCommand : ConsoleCommand
    {
        public override IAttrib[] StandardAttributes { get; protected set; } = new IAttrib[]
        {
            new AllAttribute()
        };
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
            IAttrib[] attribs = (Parser as StandardParser).GetAttributes(this, commandLineText);
            CurrentAttributes = ExtractAttributes(attribs).ToArray();
            SetParameters<NumberParameter, short>(commandLineText);
            if (IsCorrectSyntax(true))
            {
                if (attribs.Length > 0)
                {
                    attribs[0].Action();
                    Message = attribs[0].Message;
                }
                else
                    Message = ProjectManager.RemoveModule(0);
            }
            else
                Message = GetErrorMessage(commandLineText);
        }
    }
}