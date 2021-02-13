using Command.Interfaces;
using CyberpunkConsoleControl;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;

namespace CyberpunkConsole.Scripts.ViewModels
{
    /// <summary>
    /// Class to manage <see cref="CyberConsole"/> control.
    /// </summary>
    public class ConsoleViewModel: VMBase
    {
        #region Properties

        #region EnterSymbol

        private string enterSymbol = " $ ";
        /// <summary>
        /// Symbol which indicates <see cref="ConsoleMode.COMMAND_MODE"/>.
        /// </summary>
        public string EnterSymbol
        {
            get => enterSymbol;
            set
            {
                enterSymbol = value;
                OnPropertyChanged("EnterSymbol");
            }
        }

        #endregion

        #region Text

        /// <summary>
        /// Cyber console text.
        /// </summary>
        public string Text
        {
            get => Document.Text;
            set
            {
                Document.Text = value;
                OnPropertyChanged("Text");
            }
        }

        #endregion

        #region Document

        private TextDocument document = new TextDocument();
        /// <summary>
        /// Cyber console document.
        /// </summary>
        public TextDocument Document
        {
            get => document;
            set
            {
                document = value;
                OnPropertyChanged("Document");
            }
        }

        #endregion

        #endregion

        #region Commands

        #region SelectRowCommand

        private RelayCommand selectRowCommand;
        public RelayCommand SelectRowCommand
        {
            get => selectRowCommand ?? (selectRowCommand = new RelayCommand(obj =>
            {
                (obj as CyberConsole).SelectRow();
            }));
        }
        #endregion

        #region PrintHelpInfo

        private RelayCommand printHelpInfoCommand;
        public RelayCommand PrintHelpInfoCommand
        {
            get => printHelpInfoCommand ?? (printHelpInfoCommand = new RelayCommand(obj =>
            {
                List<Type> commandTypes = CommandsManager.GetCommandTypes();
                int counter = 0;
                foreach (Type t in commandTypes)
                {
                    ICommand command = (ICommand)Activator.CreateInstance(t);
                    InsertText($"'{command.Spelling}' - {command.Description}");
                    if (command.StandardAttributes != null)
                    {
                        InsertText("ATTRIBUTES:");
                        PrintParameters(command.StandardAttributes);
                    }
                    if(command.StandardParameters != null)
                    {
                        InsertText("PARAMETERS:");
                        PrintParameters(command.StandardParameters);
                    }
                    counter++;
                    if(counter != commandTypes.Count)
                        InsertText("");//go to next line.
                }
               
            }));
        }
        #endregion

        #endregion

        #region Methods
        
        private void InsertText(string value, bool isNewLine = true)
        {
            Text = Text.Insert(Text.Length, value);
            if (isNewLine)
                Text = Text.Insert(Text.Length, "\n");
        }

        private void PrintParameters(IParameter[] parameters)
        {
            foreach (IParameter param in parameters)
            {
                string paramValue = param.Value == string.Empty ? param.GetType().Name : param.Value;
                InsertText($"\t'{paramValue}' - {param.Description}");
            }
        }

        #endregion
    }
}
