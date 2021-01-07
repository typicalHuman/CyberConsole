using Command.Errors;
using Command.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Commands.StandardCommands.AddNewCommand.Attributes
{
    internal class FileAttribute: IAttrib
    {
        #region 

        #endregion

        public void Action(object[] args = null)
        {
            if (args == null || args.Length == 0)
                Error = new NullValueError();
            else if (args.Length > 1)
                Error = new ParametersExcessError("File attribute must have a single path to .cs file.");
            else
            {
                string filePath = args[0].ToString();
                ProcessStartInfo pInfo = new ProcessStartInfo();
                string arggs = string.Join(" ", "Commands", filePath);
                Process.Start(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "StandardCommands\\AddNewCommand\\AddType.exe"), 
                arggs ); 
            }
        }

        public string Value => "-f";
        public bool Equals(string parameter)
        {
            return Value.Equals(parameter);
        }

        public Error Error { get; set; }
        public int Offset { get; set; }
        public int EndOffset { get; set; }
    }
}
