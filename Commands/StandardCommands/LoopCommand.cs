using Command;
using Command.Errors;
using Command.Interfaces;
using Command.Parameters;
using Command.Parsers;
using CyberpunkConsoleControl;
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

        private const string PARSE_PARAMETERS_PATTERN = "\"([^']*)\"|" + @"\(([^']*)\)" +"|[^ ]+";

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

        public override string PrintInfo()
        {
            if(Message == string.Empty)
                return Message;
            return base.PrintInfo();
        }

        public override void Action(string commandLineText, params object[] args)
        {
            SetParameters<BracketParameter, string>(commandLineText);
            SetParameters<StringParameter, string>(commandLineText);
            CyberConsole cc = args[0] as CyberConsole;
            if (IsCorrectSyntax(true) && cc != null)
            {
                if (Parameters[0].GetType() == StandardParameters[0].GetType())
                {
                    string[] splitedOperations = Parameters[0].Value.Split(';');
                    List<IParameter> parameters = new List<IParameter>();
                    for (int i = 0; i < splitedOperations.Length; i++)
                        parameters.Add(new NumberParameter().GetParameter(splitedOperations[i]));
                    if (parameters.Count == 3)
                    {
                        if (Parameters.Length == 2)
                        {
                            string commandToExecute = Parameters[1].Value;
                            Parameters = parameters.ToArray();
                            if (IsCorrectSyntax(true))
                            {
                                int start = int.Parse(Parameters[0].Value);
                                int condition = int.Parse(Parameters[1].Value);
                                int iteration = int.Parse(Parameters[2].Value);
                                for (int i = start; i < condition; i += iteration)
                                    cc.ProcessCommand(commandToExecute);
                                Message = string.Empty;
                                return;
                            }
                        }
                        else
                        {
                            SetParametersAbsenceError();
                        }
                    }
                    else
                    {
                        Parameters = new IParameter[]
                        {
                            new NumberParameter()
                            {
                                Error = new WrongParametersCountError(" There must be three numbers: start point; condtion; iteration")
                            }
                        };
                    }
                }
                else
                {
                    Parameters[0].Error = new ParameterNotFoundError("with loop conditions"); 
                }
            }
            Message = GetErrorMessage(commandLineText);
        }


    }
}
