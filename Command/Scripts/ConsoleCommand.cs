using Command.Errors;
using Command.Interfaces;
using Command.Parsers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Command
{
    /// <summary>
    /// Abstract class for creating commands.
    /// </summary>
    public abstract class ConsoleCommand: ICommand
    {
        #region Properties

        #region Protected

        /// <summary>
        /// Parser which understands command syntax.
        /// </summary>
        protected virtual IParser Parser { get; set; } = new StandardParser();

        /// <summary>
        /// Message to write in the end of action.
        /// </summary>
        protected virtual string Message { get; set; }

        #endregion

        #region Public

        public abstract IAttrib[] StandardAttributes { get; protected set; }

        public abstract IAttrib[] CurrentAttributes { get; set; }

        public abstract IParameter[] Parameters { get; set; }

        public abstract string Spelling { get; protected set; }


        #endregion

        #endregion

        #region Methods

        #region To override methods

        public abstract void Action(string commandLineText, params object[] args);

        public virtual string PrintInfo()
        {
            return "";
        }

        /// <summary>
        /// Is <paramref name="commandLineText"/> suited for command lexic.
        /// </summary>
        public virtual bool IsCommandLexic(string commandLineText)
        {
            return Parser.IsCommandLexic(Spelling, commandLineText);
        }

        #endregion

        #region Protected

        #region SetParameters

        /// <summary>
        /// Set parameters of <typeparamref name="T"/> type.
        /// </summary>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <typeparam name="U">Parameter value type.</typeparam>
        /// <param name="commandLineText">Line to parse.</param>
        protected void SetParameters<T, U>(string commandLineText) where T : Parameter<U>, new()
        {
            List<string> parameters = GetStringParameters(commandLineText).ToList();
            InitializeParametersArray(parameters.Count);
            int LengthToSum = 0;
            for (int i = 0, k = 0; i < parameters.Count; i++, k++)
            {
                if (IsSetParameter(k))//check error
                {
                    SetParameterValue<T, U>(ref Parameters[k], parameters[i], commandLineText, LengthToSum);
                    commandLineText = commandLineText.Remove(Parameters[k].Offset - LengthToSum, parameters[i].Length);
                    LengthToSum += parameters[i].Length;
                    parameters.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Does the parameter need to be reseted.
        /// </summary>
        /// <param name="index">Index of parameter.</param>
        /// <returns>Will the parameter be reseted.</returns>
        private bool IsSetParameter(int index)
        {
            return Parameters[index] == null ||
                   (Parameters[index].Error != null &&
                    Parameters[index].Error.ErrorType == ErrorTypes.ANOTHER_PARAMETER);
        }

        /// <summary>
        /// Set parameter value and offset.
        /// </summary>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <typeparam name="U">Parameter value type.</typeparam>
        /// <param name="param">Parameter to set.</param>
        /// <param name="value">String value of parameter.</param>
        /// <param name="commandLineText">Line to parse.</param>
        /// <param name="LengthToSum">Count of removed chars from the <paramref name="commandLineText"/>.</param>
        private void SetParameterValue<T, U>(ref IParameter param, string value, string commandLineText, int LengthToSum) where T : Parameter<U>, new()
        {
            param = new T().GetParameter(value);
            param.Offset = commandLineText.IndexOf(value) + LengthToSum;
            param.EndOffset = param.Offset + value.Length;
        }

        /// <summary>
        /// Initializes array if it's empty.
        /// </summary>
        /// <param name="count">Count of the parameters values.</param>
        private void InitializeParametersArray(int count)
        {
            if (Parameters == null || Parameters.Length == 0)
                Parameters = new IParameter[count];
        }

        /// <summary>
        /// Get values of parameters in the <paramref name="commandLineText"/>.
        /// </summary>
        /// <param name="commandLineText">Line to parse.</param>
        /// <returns>Array of parameters values.</returns>
        private string[] GetStringParameters(string commandLineText)
        {
            return ((StandardParser)Parser).GetParameters(commandLineText, StandardAttributes)
                                                          .ToArray();
        }


        #endregion

        #region GetErrorMessage

        /// <summary>
        /// Get the message which is consisted of all errors messages of <paramref name="commandLineText"/>.
        /// </summary>
        /// <param name="commandLineText">Line to parse.</param>
        /// <returns>String with all errors messages.</returns>
        protected string GetErrorMessage(string commandLineText)
        {
            Error[] errors = GetErrors(commandLineText);
            StringBuilder message = new StringBuilder();
            for (int i = 0; i < errors.Length; i++)
            {
                if (errors[i].Offset != default(int))
                    message.Append($"c:{errors[i].Offset}; ");// c: means column (offset alternative).
                message.Append(errors[i].Message);
                if (i != errors.Length - 1)//if it's not last error - insert new line for next error.
                    message.Append("\n");
            }
            return message.ToString();
        }

        /// <summary>
        /// Get array of errors in <paramref name="commandLineText"/>.
        /// </summary>
        /// <param name="commandLineText">Line to parse.</param>
        /// <returns>Array with errors or array with the single <see cref="SyntaxError"/> error.</returns>
        private Error[] GetErrors(string commandLineText)
        {
            if (Parameters.Length > 0 && Parameters[0] != null)
            {
                int errorsCount = Parameters.Count(p => p.Error != null && p.Error.ErrorType != 0);
                Error[] errors = new Error[errorsCount];
                int LengthToSum = 0;
                for (int i = 0, k = 0; i < Parameters.Length; i++)
                {
                    if (Parameters[i].Error.ErrorType != 0)
                    {
                        Parameters[i].Error.Offset = GetErrorOffset(ref LengthToSum, i, ref commandLineText);
                        errors[k] = Parameters[i].Error;
                        k++;
                    }
                }
                return errors;
            }
            return new[] { new SyntaxError() };
        }

        //TODO: set error offset for quote, not for start of parameter.
        /// <summary>
        /// Get offset of error.
        /// </summary>
        /// <param name="LengthToSum">Count of removed chars from the <paramref name="commandLineText"/>.</param>
        /// <param name="i">Iteration index.</param>
        /// <param name="commandLineText">Line to parse.</param>
        /// <returns>Error offset.</returns>
        private int GetErrorOffset(ref int LengthToSum, int i, ref string commandLineText)
        {
            int index = 0;
            while (index < Parameters[i].Offset)
            {
                index = commandLineText.IndexOf(Parameters[i].Value) + LengthToSum;
                commandLineText = commandLineText.Remove(index - LengthToSum, Parameters[i].Value.Length);
                LengthToSum += Parameters[i].Value.Length;
            }
            return index;
        }


        #endregion

        #region IsCorrectSyntax

        /// <summary>
        /// Is correct parameters syntax.
        /// </summary>
        /// <returns>Is the parameters syntax fits the expected parameters.</returns>
        protected bool IsCorrectSyntax()
        {
            return Parameters?.Where(p => p != null && p.Error != null && p.Error.ErrorType != 0)
                          .Count() == 0;
        }

        #endregion

        #endregion

        #endregion
    }
}
