using Command;
using Command.Attributes;
using Command.Interfaces;
using Command.Parsers;
using CyberpunkConsoleControl;

namespace StandardCommands
{
    class EditorCommand: ConsoleCommand
    {
        public override IAttrib[] StandardAttributes { get; protected set; } = new[] { new SaveAttribute() };
        public override IAttrib[] CurrentAttributes { get; set; }
        public override IParameter[] Parameters { get; set; }
        public override string Spelling { get; protected set; } = "echo";

        public override string PrintInfo()
        {
            return "Mode is changed to editor\n";
        }

        public override void Action(string commandLineText, params object[] args)
        {
            CurrentAttributes = (Parser as StandardParser).GetAttributes(this, commandLineText);
            CyberConsole cc = args[0] as CyberConsole;
            if(cc != null && IsCorrectSyntax())
            {
                cc.EnterSymbol = " > ";
                cc.ConsoleMode = ConsoleMode.EDITOR_MODE;
            }
        }


    }
}
