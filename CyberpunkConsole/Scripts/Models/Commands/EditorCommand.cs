using Command;
using Command.Attributes;
using Command.Interfaces;
using Command.Parsers;

namespace CyberpunkConsole.Scripts.Models.Commands
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

        public override void Action(string commandLineText)
        {
            CurrentAttributes = (Parser as StandardParser).GetAttributes(this, commandLineText);
            if(commandLineText.Length < 7)
            {
                App.ConsoleVM.EnterSymbol = " > ";
                App.ConsoleVM.ConsoleMode = ConsoleMode.EDITOR_MODE;
            }
        }
    }
}
