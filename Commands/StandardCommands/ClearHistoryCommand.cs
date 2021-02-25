using Command;
using Command.Interfaces;
using CyberpunkConsoleControl;
using System.IO;

namespace Commands.StandardCommands
{
    class ClearHistoryCommand: ConsoleCommand
    {
        #region Constants

        private const string HISTORY_FILE_NAME = "history.json";

        #endregion

        public override IAttrib[] StandardAttributes { get; protected set; }
        public override IAttrib[] CurrentAttributes { get; set; }
        public override IParameter[] StandardParameters { get; protected set; }
        public override IParameter[] Parameters { get; set; }
        public override string Spelling { get; protected set; } = "clr_hist";
        protected override string Message { get; set; } = "History is clear.";

        public override void Action(string commandLineText, params object[] args)
        {
            commandLineText = commandLineText.Replace(" ", "");
            CyberConsole cc = (args[0] as CyberConsole);
            if (commandLineText.Length == Spelling.Length)
            {
                if (cc != null)
                {
                    cc.PreviousCommands.Clear();
                    if (File.Exists(HISTORY_FILE_NAME))
                    {
                        try
                        {
                            FileStream fs = File.Create(HISTORY_FILE_NAME);
                            fs.Close();
                            File.Delete(HISTORY_FILE_NAME);
                        }
                        //The process cannot acess the file.
                        catch (IOException)
                        {
                        }
                    }

                    Message = "History is clear.";
                }
                else
                    Message = "CyberConsole argument was null.";
            }
            else
                Message = "Excess symbols in command.";
        }
    }
}
