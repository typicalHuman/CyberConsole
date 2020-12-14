using CyberpunkConsoleControl;
using ICSharpCode.AvalonEdit.Document;

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

        #region CyberConsole

        private CyberConsole cyberConsole = new CyberConsole();
        /// <summary>
        /// Cyber console control.
        /// </summary>
        public CyberConsole CyberConsole
        {
            get => cyberConsole;
            set
            {
                cyberConsole = value;
                OnPropertyChanged("CyberConsole");
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
    }
}
