using Command.Errors;
using Command.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Command.Attributes
{
    internal class ErrorAttribute: IAttrib
    {
        public void Action(object input = null)
        {
        }

        public string Value { get; set; } = null;

        public bool Equals(string parameter) => false;

        public Error Error { get; set; }
        public int Offset { get; set; }
        public int EndOffset { get; set; }
    }
}
