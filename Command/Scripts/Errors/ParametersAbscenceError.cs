using Command.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Command.Errors
{
    public class ParametersAbscenceError: Error
    {
        public ParametersAbscenceError(params IParameter[] standardParameter)
        {
            if(standardParameter.Length > 0)
            {
                StringBuilder sb = new StringBuilder(Message);
                sb.Append(" For example: ");
                for (int i = 0; i < standardParameter.Length - 1; i++)
                {
                    sb.Append(standardParameter[i].GetType().Name + ", ");
                }
                sb.Append(standardParameter[standardParameter.Length - 1].GetType().Name + ";");
                Message = sb.ToString();
            }
        }
        public override ErrorTypes ErrorType { get; protected set; } = ErrorTypes.WRONG_PARAMETERS_COUNT;
        public override string Message { get; protected set; } = "Command needs any parameters.";
    }
}
