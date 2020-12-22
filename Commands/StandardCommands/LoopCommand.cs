using Command;
using Command.Errors;
using Command.Interfaces;
using Command.Parameters;
using Command.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands.StandardCommands
{
    class LoopCommand : ConsoleCommand
    {
        #region Constants

        private const string PARSE_PARAMETERS_PATTERN = "\"([^']*)\"|[^ ]+";

        #endregion

        public LoopCommand()
        {
            Parser = new StandardParser(PARSE_PARAMETERS_PATTERN);
        }

        public override IAttrib[] StandardAttributes { get; protected set; }
        public override IAttrib[] CurrentAttributes { get; set; }
        public override IParameter[] StandardParameters { get; protected set; } =
        {
            new BracketParameter(),
            new StringParameter()
        };
        public override IParameter[] Parameters { get; set; }
        public override string Spelling { get; protected set; } = "for";

        public override void Action(string commandLineText, params object[] args)
        {
            SetParameters<BracketParameter, string>(commandLineText);
            SetParameters<StringParameter, string>(commandLineText);
            if (IsCorrectSyntax(true))
            {
                if (Parameters[0] == StandardParameters[0])
                {
                    string[] splitedOperations = Parameters[0].Value.Split(';');
                    List<IParameter> parameters = new List<IParameter>();
                    for (int i = 0; i < splitedOperations.Length; i++)
                        parameters.Add(new NumberParameter().GetParameter(splitedOperations[i]));
                }
                else
                {
                    Parameters[0].Error = new ParameterNotFoundError("with loop conditions"); 
                }
            }
            if (Parameters.Length == 0)
            {
                Parameters = new IParameter[]{ new BracketParameter() { Error = new ParametersAbscenceError(StandardParameters), Value = string.Empty } };
            }
            Message = GetErrorMessage(commandLineText);
        }


    }
}
