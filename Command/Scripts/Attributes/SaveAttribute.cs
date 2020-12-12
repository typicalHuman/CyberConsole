using Command.Errors;
using Command.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Command.Attributes
{
    public class SaveAttribute: IAttrib
    {
        public void Action(object input)
        {
            if (input == null)
            {
                Error = new NullValueError();
            }
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                if (saveFileDialog.ShowDialog() == true)
                    File.WriteAllText(saveFileDialog.FileName, input.ToString());
            }
        }

        public string Value { get; set; } = "-s";

        public bool Equals(string parameter)
        {
            return parameter.Equals(Value);
        }

        public Error Error { get; set; }
        public int Offset { get; set; }
        public int EndOffset { get; set; }
    }
}
