using Command;
using Command.Attributes;
using Command.Interfaces;
using Command.Parsers;
using CyberpunkConsoleControl;
using System.Linq;
using System.Windows.Input;

namespace StandardCommands
{
    class EditorCommand: ConsoleCommand
    {
        #region Properties

        #region Overrided
        public override IAttrib[] StandardAttributes { get; protected set; } = new[] { new SaveAttribute() };
        public override IAttrib[] CurrentAttributes { get; set; }
        public override IParameter[] StandardParameters { get; protected set; }
        public override IParameter[] Parameters { get; set; }
        public override string Spelling { get; protected set; } = "echo";

        #endregion

        #endregion

        #region Methods

        #region Overrided

        public override string PrintInfo()
        {
            return Message;
        }


        public override void Action(string commandLineText, params object[] args)
        {
            Parameters = (Parser as StandardParser).GetAttributes(this, commandLineText);
            CurrentAttributes = ExtractAttributes(Parameters).ToArray();
            CyberConsole cc = args[0] as CyberConsole;
            if (cc != null && IsCorrectSyntax())
            {
                cc.EnterSymbol = " > ";
                cc.ConsoleMode = ConsoleMode.EDITOR_MODE;
                cc.PreviewKeyDown += OnPreviewKeyDown;
                Message = "Mode is changed to editor.\n";
            }
            else
                Message = GetErrorMessage(commandLineText) + "\n";
        }


        #endregion

        #endregion

        #region Events

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            CyberConsole cc = sender as CyberConsole;
            if (cc.ConsoleMode == ConsoleMode.EDITOR_MODE &&
                (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (Keyboard.IsKeyDown(Key.LeftShift) && Keyboard.IsKeyDown(Key.C))
                {
                    cc.ConsoleMode = ConsoleMode.COMMAND_MODE;
                    (cc.TextArea.LeftMargins[0] as NewLineMargin).UpdateLineStates(cc.ConsoleMode);
                    cc.Text = cc.Text.Insert(cc.Text.Length, "\nMode is changed to console.\n");
                    cc.TextArea.Caret.Line = cc.Document.Lines.Count;
                    cc.PreviewKeyDown -= OnPreviewKeyDown;
                }
            }
        }

        #endregion





    }
}
