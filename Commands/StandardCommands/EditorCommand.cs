using Command;
using Command.Attributes;
using Command.Interfaces;
using Command.Parameters;
using Command.Parsers;
using CyberpunkConsoleControl;
using System.Linq;

namespace StandardCommands
{
    class EditorCommand: ConsoleCommand
    {
        public override IAttrib[] StandardAttributes { get; protected set; } = new[] { new SaveAttribute() };
        public override IAttrib[] CurrentAttributes { get; set; }
        public override IParameter[] StandardParameters { get; protected set ; }
        public override IParameter[] Parameters { get; set; }
        public override string Spelling { get; protected set; } = "echo";

        public override string PrintInfo()
        {
            return "Mode is changed to editor\n";
        }

        public override void Action(string commandLineText, params object[] args)
        {
            Parameters = (Parser as StandardParser).GetAttributes(this, commandLineText);
            CurrentAttributes = ExtractAttributes(Parameters).ToArray();
            CyberConsole cc = args[0] as CyberConsole;
            if(cc != null && IsCorrectSyntax())
            {
                cc.EnterSymbol = " > ";
                cc.ConsoleMode = ConsoleMode.EDITOR_MODE;
            }
        }

       
    }
}
