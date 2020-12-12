using CyberpunkConsole.Scripts.Models;
using CyberpunkConsole.Scripts.Views;
using ICSharpCode.AvalonEdit.Document;

namespace CyberpunkConsole.Scripts.ViewModels
{
    /// <summary>
    /// Class to manage <see cref="CyberConsole"/> control.
    /// </summary>
    public class ConsoleViewModel: VMBase
    {
        #region Properties


        #region ConsoleMode

        private ConsoleMode consoleMode = ConsoleMode.COMMAND_MODE;
        /// <summary>
        /// Indicator of CyberConsole regime.
        /// </summary>
        public ConsoleMode ConsoleMode
        {
            get => consoleMode;
            set
            {
                consoleMode = value;
                OnPropertyChanged("ConsoleMode");
            }
        }

        #endregion

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

        #endregion

        #region Methods

        /// <summary>
        /// Insert <paramref name="textToInsert"/> in Console.
        /// </summary>
        /// <param name="textToInsert">Text which will be inserted.</param>
        /// <param name="isNewLine">Insert with addition of new line.</param>
        public void InsertText(string textToInsert, bool isNewLine = false)
        {
            Text = Text.Insert(Text.Length, $"{textToInsert}");
            if (isNewLine)
                Text = Text.Insert(Text.Length, "\n");
        }

        #endregion
    }
}
