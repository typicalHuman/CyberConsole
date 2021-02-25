using Command;
using Command.Interfaces;
using Command.StandardParameters;
using System.Linq;
using System.ComponentModel;
using Commands.StandardCommands.AddNewCommand.Attributes;
using Command.Parsers;
using Command.Errors;
using System.Threading.Tasks;
using CyberpunkConsoleControl;
using ICSharpCode.AvalonEdit.Editing;
using System;

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
            new StringParameter(), new QuoteStringParameter()
        };
        public override IParameter[] Parameters { get; set; }
        public override string Spelling { get; protected set; } = "add_cmnd";

        public override string Description { get; protected set; } = "command for dynamically adding new commands from files;";


        public override async void Action(string commandLineText, params object[] args)
        {
            CyberConsole cc = (args[0] as CyberConsole);
            int start = cc.Document.LineCount;
            cc.InsertText("Building . . .");
            cc.IsEnabled = false;
            await Task.Run(() =>
            {
                IAttrib[] attribs = (Parser as StandardParser).GetAttributes(this, commandLineText);
                CurrentAttributes = ExtractAttributes(attribs).ToArray();
                SetParameters<StringParameter, string>(commandLineText);
                SetParameters<QuoteStringParameter, string>(commandLineText);
                if (IsCorrectSyntax(true))
                {
                    if (IsCorrectParameters())
                    {
                        if (CurrentAttributes.Length > 1)
                        {
                            Message = new ParametersExcessError("File attribute must have a single path to .cs file.").Message;
                            return;
                        }
                        else if (CurrentAttributes.Length == 0)
                            CurrentAttributes = new IAttrib[] { new FileAttribute() };
                        (CurrentAttributes[0] as FileAddAttribute).Action(
                            Parameters.OfType<StringParameter>().Select(p => p.Value).ToArray(), //Get files.
                            Parameters.FirstOrDefault(p => p.GetType() == typeof(QuoteStringParameter))?.Value);//Get name.
                        Message = CurrentAttributes[0].Message;
                    }
                    else
                        Message = "Module should contain only one name.";
                }
                else
                    Message = GetErrorMessage(commandLineText);
                cc.Dispatcher.Invoke(() =>
                {
                    cc.InsertText(Message, true);
                    int end = cc.Document.LineCount - 1;
                    (cc.TextArea.LeftMargins[0] as NewLineMargin).RemoveLines(start, end);
                    cc.IsEnabled = true;
                    cc.Focus();
                });
            });
        }
        private bool IsCorrectParameters()
        {
            if (Parameters.Where(p => p.GetType() == typeof(QuoteStringParameter)).Count() > 1)
                return false;
            return true;
        }
    }
}
