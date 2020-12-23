using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Command.Errors
{
    public class ParametersExcessError: Error
    {
        /// <summary>
        /// Initializes <see cref="ParametersExcessError"/> error.
        /// </summary>
        /// <param name="additionalInstruction">Instruction which describes a logic of the parameters input.</param>
        public ParametersExcessError(string additionalInstruction = "")
        {
            Message = String.Concat(Message, additionalInstruction);
        }
        public override ErrorTypes ErrorType { get; protected set; } = ErrorTypes.WRONG_PARAMETERS_COUNT;
        public override string Message { get; protected set; } = $"Too many arguments! ";
    }
}
