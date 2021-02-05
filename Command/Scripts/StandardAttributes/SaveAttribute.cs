using Command.Errors;
using Command.Interfaces;
using Microsoft.Win32;
using System.IO;

namespace Command.StandardAttributes
{
    /// <summary>
    /// Attribute for saving content on file.
    /// </summary>
    public class SaveAttribute: IAttrib
    {
        #region Properties

        #region Private

        private string Filter { get; set; }

        #endregion

        #region Public 
        public string Value { get; set; } = "-s";

        public string Message { get; private set; }

        public Error Error { get; set; }
        public int Offset { get; set; }
        public int EndOffset { get; set; }

        public virtual string Description { get; protected set; } = "'-s' - attribute for saving object into .txt (or any other) files;";

        #endregion

        #endregion

        /// <summary>
        /// Ctor for initializing saving filter.
        /// By default: text files and all files.
        /// </summary>
        public SaveAttribute(string filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*")
        {
            Filter = filter;
        }

        public void Action(object[] args)
        {
            if (args == null || args.Length < 1)
            {
                Error = new NullValueError();
            }
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = Filter;
                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveFileDialog.FileName, args[0].ToString());
                }
                
            }
        }

        public bool Equals(string parameter)
        {
            return parameter.Equals(Value);
        }

   
    }
}
