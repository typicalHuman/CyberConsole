using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands.StandardCommands.AddNewCommand.ProjectManager
{
    internal class Module
    {
        public string[] FilesPaths { get; private set; }
        public string[] DllsPaths { get; private set; }
        public string Name { get; private set; }

        public Module(string[] files, string[] dlls, string name)
        {
            FilesPaths = files;
            DllsPaths = dlls;
            Name = name;
        }
    }
}
