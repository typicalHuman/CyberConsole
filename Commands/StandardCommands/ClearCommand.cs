using Command;
using Command.Errors;
using Command.Interfaces;
using CyberpunkConsoleControl;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace Commands.StandardCommands
{
    class ClearCommand: ConsoleCommand
    {
        #region Properties
        #region Overrided
        public override IAttrib[] StandardAttributes { get; protected set; }
        public override IAttrib[] CurrentAttributes { get; set; }
        public override IParameter[] StandardParameters { get; protected set; }
        public override IParameter[] Parameters { get; set; } = new IParameter[0];
        public override string Spelling { get; protected set; } = "clear";
        #endregion
        #endregion


        #region Methods

        #region Overrided

        public override string PrintInfo()
        {
            if (Message != string.Empty)
                Message += "\n";
            return Message;
        }

        public override void Action(string commandLineText, params object[] args)
        {
            CyberConsole cc = args[0] as CyberConsole;
            commandLineText = commandLineText.Replace(" ", "");
            if (IsCorrectSyntax() && cc != null && commandLineText.Length == Spelling.Length)
            {
                cc.Text = string.Empty;
                (cc.TextArea.LeftMargins[0] as NewLineMargin).Clear();
                (cc.TextArea.ReadOnlySectionProvider as TextSegmentReadOnlySectionProvider<TextSegment>).Segments.Clear();
            }
            else if(cc == null)
                Message = new NullArgumentError("CyberConsole").Message ;
            else if (commandLineText.Length != Spelling.Length)
                Message = new ParametersExcessError("This command shouldn't have any arguments.").Message ; 
            else
                Message = new SyntaxError().Message;
        }

        #endregion

        #endregion

    }
}
