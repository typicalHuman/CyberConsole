using Command;
using Command.Interfaces;
using Command.Parsers;
using System.Linq;
using Command.StandardParameters;
using Commands.StandardCommands.RemoveCommandCmnd.Attributes;
using System;

namespace Commands.StandardCommands
{
    class RemoveCommand : ConsoleCommand
    {
        public override IAttrib[] StandardAttributes { get; protected set; } = new IAttrib[]
        {
            new AllAttribute()
        };
        public override IAttrib[] CurrentAttributes { get; set; }
        public override IParameter[] StandardParameters { get; protected set; } = new IParameter[]
        {
            new NumberParameter(), new QuoteStringParameter()
        };
        public override IParameter[] Parameters { get; set; }
        public override string Spelling { get; protected set; } = "rm_cmnd";
        public override string Description { get; protected set; } = "remove command by index;";

        public override void Action(string commandLineText, params object[] args)
        {
            IAttrib[] attribs = (Parser as StandardParser).GetAttributes(this, commandLineText);
            CurrentAttributes = ExtractAttributes(attribs).ToArray();
            SetParameters<NumberParameter, short>(commandLineText);
            SetParameters<QuoteStringParameter, string>(commandLineText);
            if (IsCorrectSyntax(true))
            {
                if (IsCorrectParameters())
                {
                    if (attribs.Length > 0)
                    {
                        attribs[0].Action(Parameters.Select(p => p.Value).ToArray());
                        Message = attribs[0].Message;
                    }
                    else
                    {
                        Message = ProjectManager.RemoveModule(0);
                    }
                }
                else
                    Message = "Parameters must contain only 1 type of module search (search by indexes or search by names);";
            }
            else
                Message = GetErrorMessage(commandLineText);
        }

        private bool IsCorrectParameters()
        {
            if(Parameters.Length > 1)
            {
                Type t = Parameters.First().GetType();
                foreach(IParameter param in Parameters)
                {
                    if (t != param.GetType())
                        return false;
                }
            }
            return true;
        }
    }
}