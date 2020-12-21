using Command;
using Command.Interfaces;
using Command.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands.StandardCommands
{
    class LoopCommand : ConsoleCommand
    {
        public override IAttrib[] StandardAttributes { get; protected set; }
        public override IAttrib[] CurrentAttributes { get; set; }
        public override IParameter[] StandardParameters { get; protected set; } =
        {
            new BracketParameter()
        };
        public override IParameter[] Parameters { get; set; }
        public override string Spelling { get; protected set; } = "for";

        public override void Action(string commandLineText, params object[] args)
        {
            SetParameters<BracketParameter, string>(commandLineText);
            var a = Parameters;
        }
    }
}
