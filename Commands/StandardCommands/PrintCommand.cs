using Command;
using Command.Interfaces;
using Command.StandardParameters;
using Command.Errors;
using System.Linq;
using System;

namespace StandardCommands
{
    class PrintCommand: ConsoleCommand 
    {
        #region Constants

        private const string STRING_EXCESS_MESSAGE = "Input only 1 string parameter.";
        private const string NUMBER_EXCESS_MESSAGE = "Input only 1 number parameter.";

        #endregion

        #region Properties

        #region Overrided
        public override string Spelling { get; protected set; } = "print";
        public override IAttrib[] StandardAttributes { get; protected set; }
        public override IAttrib[] CurrentAttributes { get; set; }
        public override IParameter[] StandardParameters { get; protected set; } = 
        {
            new NumberParameter(),
            new StringParameter()
        };
        public override IParameter[] Parameters { get; set; }
        public override string Description { get; protected set; } = "command for printing info on the screen;\n"+
                                                                      "Example - print 5 \"Hey\" - this construction will print 'Hey' 5 times;";
        #endregion

        #endregion

        #region Methods

        #region Overrided

        #region PrintInfo

        public override string PrintInfo()
        {
            return Message;
        }

        #endregion

        #region Action

        public override void Action(string commandLineText, params object[] args)
        {
            SetParameters<NumberParameter, short>(commandLineText);
            SetParameters<StringParameter, string>(commandLineText);
            CheckParameters();
            if (IsCorrectSyntax(true))
            {
                int iterationsCount = 1;
                Type numberParameter = typeof(NumberParameter);
                if (Parameters.Select(t => t.GetType()).Contains(numberParameter))
                {
                    iterationsCount = int.Parse(Parameters
                        .FirstOrDefault(p => p.GetType() == numberParameter)
                        .Value);
                }
                for (int i = 0; i < iterationsCount; i++)
                {
                    foreach (IParameter parameter in Parameters.Where(p => p.GetType() == typeof(StringParameter)))
                        Message = Message.Insert(Message.Length, $"{parameter.Value}\n");
                }
            }
            else
                Message = $"{GetErrorMessage(commandLineText)}\n";
        }


        private void CheckExcessParameters()
        {
            int stringParametersCount = 0;
            int numberParametersCount = 0;
            foreach (IParameter parameter in Parameters)
            {
                if (numberParametersCount < 2 && parameter.GetType() == typeof(NumberParameter))
                {
                    numberParametersCount++;
                    if (numberParametersCount > 1)
                    {
                        parameter.Error = new ParametersExcessError(NUMBER_EXCESS_MESSAGE);
                        return;
                    }
                }
                else if (stringParametersCount < 2)
                {
                    stringParametersCount++;
                    if (stringParametersCount > 1)
                    {
                        parameter.Error = new ParametersExcessError(STRING_EXCESS_MESSAGE);
                        return;
                    }
                }
            }
        }

        private void CheckStringParameterExist()
        {
            if (!Parameters.Select(p => p.GetType())
                           .ToList()
                           .Contains(typeof(StringParameter)))
                SetParametersAbsenceError();
        }

        private void CheckParameters()
        {
            CheckExcessParameters();
            CheckStringParameterExist();
        }

        #endregion

        #region MyRegion

        protected override void SetParametersAbsenceError()
        {
            Parameters = new IParameter[]
            {
                new NumberParameter()
                {
                    Error = new ParametersAbscenceError(StandardParameters[1]),
                    Value = string.Empty
                }
           };
        }

        #endregion

        #endregion

        #endregion



    }
}
