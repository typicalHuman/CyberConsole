using Command;
using Command.Errors;
using Command.Interfaces;
using Command.StandardParameters;
using Command.Parsers;
using CyberpunkConsoleControl;
using System.Collections.Generic;
using System.Linq;

namespace Commands.StandardCommands
{
    class LoopCommand : ConsoleCommand
    {
        #region Constants

        private const string PARSE_PARAMETERS_PATTERN = "\"([^']*)\"|" + @"\(([^']*)\)" +"|[^ ]+";

        //
        #endregion

        #region Ctor
        public LoopCommand()
        {
            Parser = new StandardParser(PARSE_PARAMETERS_PATTERN);
        }
        #endregion

        #region Properties

        #region Overrided
        public override IAttrib[] StandardAttributes { get; protected set; }
        public override IAttrib[] CurrentAttributes { get; set; }
        public override IParameter[] StandardParameters { get; protected set; } =
        {
            new BracketParameter(),
            new StringParameter()
        };
        public override IParameter[] Parameters { get; set; }
        public override string Spelling { get; protected set; } = "for";
        public override string Description { get; protected set; } = "'for' - command for executing command several times in a row;\n"+
                                                                     "Example - for(1;11;1) \"print \"Hello\"\" - this construction will print 'Hello' 10 times;";
        #endregion

        #endregion

        #region Methods

        #region Overrided

        public override string PrintInfo()
        {
            if (Message == string.Empty)
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
                    IList<IParameter> parameters = GetCondtionParameters();
                    if (parameters.Count == 3)//3 numbers in loop brackets (num;num;num)
                    {
                        if (Parameters.Length == 2)//2 parameters: brackets with condition and command in quotes to repeat.
                            ExecuteCommandInQuotes(cc, parameters);
                        else
                            SetParametersAbsenceError();
                    }
                    else
                        Parameters = new IParameter[]
                        {
                            new NumberParameter()
                            {
                                Error = new WrongParametersCountError(" There must be three numbers: start point; condtion; iteration")
                            }
                        };
                }
                else
                    Parameters[0].Error = new ParameterNotFoundError("with loop conditions");
            }
            Message = GetErrorMessage(commandLineText);
        }


        /// <summary>
        /// Get numbers in loop brackets.
        /// </summary>
        private IList<IParameter> GetCondtionParameters()
        {
            string[] splitedOperations = Parameters[0].Value.Split(';');
            List<IParameter> parameters = new List<IParameter>();
            for (int i = 0; i < splitedOperations.Length; i++)
                parameters.Add(new NumberParameter().GetParameter(splitedOperations[i]));
            return parameters;
        }

        /// <summary>
        /// Execute command by string in quotes.
        /// </summary>
        /// <param name="cc">Console for artificial command executing.</param>
        /// <param name="parameters">Number parameters of loop condition.</param>
        private void ExecuteCommandInQuotes(CyberConsole cc, IList<IParameter> parameters)
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

        #endregion

        #endregion




    }
}
