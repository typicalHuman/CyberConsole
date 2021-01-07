using Command;
using Command.Interfaces;
using Command.StandardParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands.AddNewCommand
{
    class AddCommand : ConsoleCommand
    {
        public override IAttrib[] StandardAttributes { get; protected set; }
        public override IAttrib[] CurrentAttributes { get; set; }
        public override IParameter[] StandardParameters { get; protected set; } = new IParameter[]
        {
            new StringParameter()
        };
        public override IParameter[] Parameters { get; set; }
        public override string Spelling { get; protected set; } = "add_cmnd";

        public override void Action(string commandLineText, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
