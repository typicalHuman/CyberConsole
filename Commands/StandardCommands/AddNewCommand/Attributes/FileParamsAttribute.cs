﻿using Command.Errors;
using Command.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands.StandardCommands.AddNewCommand.Attributes
{
    internal class FileParamsAttribute: IAttrib
    {
        public void Action(object[] args = null)
        {

        }

        public string Value => "-fp";
        public bool Equals(string parameter)
        {
            return Value.Equals(parameter);
        }

        public Error Error { get; set; }
        public int Offset { get; set; }
        public int EndOffset { get; set; }
    }
}