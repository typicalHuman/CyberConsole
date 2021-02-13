using Command;
using Command.Interfaces;
using Command.StandardParameters;
using System.Linq;
using Commands.StandardCommands.AddNewCommand.Attributes;
using CyberpunkConsoleControl;
using Command.Parsers;
using Command.Errors;

namespace Commands.StandardCommands
{
    class AddCommand : ConsoleCommand
    {
        public override IAttrib[] StandardAttributes { get; protected set; } = new IAttrib[]
        {
            new FileAttribute(),
            new DirectoryAttribute()
        };
        public override IAttrib[] CurrentAttributes { get; set; }
        public override IParameter[] StandardParameters { get; protected set; } = new IParameter[]
        {
            new StringParameter()
        };
        public override IParameter[] Parameters { get; set; }
        public override string Spelling { get; protected set; } = "add_cmnd";

        public override string Description { get; protected set; } = "command for dynamically adding new commands from files;";

        public override void Action(string commandLineText, params object[] args)
        {
            IAttrib[] attribs = (Parser as StandardParser).GetAttributes(this, commandLineText);
            CurrentAttributes = ExtractAttributes(attribs).ToArray();
            SetParameters<StringParameter, string>(commandLineText);
            
            CyberConsole cc = args[0] as CyberConsole;
            if (cc != null && IsCorrectSyntax(true))
            {
                if (CurrentAttributes.Length > 1)
                {
                    Message = new ParametersExcessError("File attribute must have a single path to .cs file.").Message;
                    return;
                }
                else if (CurrentAttributes.Length == 0)
                    CurrentAttributes = new IAttrib[] { new FileAttribute() };
                CurrentAttributes[0].Action(Parameters.Select(p => p.Value).ToArray());
                Message = CurrentAttributes[0].Message;
            }
            else
                Message = GetErrorMessage(commandLineText);

        }
    }
}
