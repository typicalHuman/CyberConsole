using Command.Errors;
using Command.Interfaces;
using System;
using System.Collections.Generic;

namespace Command
{
    /// <summary>
    /// Abstract class for creating command parameter.
    /// </summary>
    /// <typeparam name="T">Parameter's value type.</typeparam>
    public abstract class Parameter<T> : IParameter
    {
        public virtual string Value { get; set; } = string.Empty;

        public int Offset { get; set; }

        public int EndOffset { get; set; }

        /// <summary>
        /// Parser which will define parameter lexic in command.
        /// </summary>
        protected abstract IParameterParser<T> Parser { get; set; }

        public Error Error { get; set; }

        /// <summary>
        /// Equals logic.
        /// </summary>
        public bool Equals(string parameter)
        {
            return Value.Equals(parameter);
        }

        /// <summary>
        /// Convert to IParameter with setting <paramref name="parameterText"/> value and defining errors.
        /// </summary>
        /// <param name="parameterText">Value to set.</param>
        /// <returns>IParameter equivalent of this parameter.</returns>
        public virtual IParameter GetParameter(string parameterText)
        {
            T value = Parser.Parse(parameterText, out Error error);
            if (!EqualityComparer<T>.Default.Equals(value, default(T)))//if value isn't default
                Value = value.ToString();
            else
                Value = parameterText;
            Error = error;
            return this;
        }
    }
}
