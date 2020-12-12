using Command.Errors;

namespace Command.Interfaces
{
    /// <summary>
    /// Parser for command's parameters.
    /// </summary>
    /// <typeparam name="T">Type of parameters values to parse.</typeparam>
    public interface IParameterParser<T>
    {
        /// <summary>
        /// Parse parameter's value.
        /// </summary>
        /// <param name="input">Value to parse.</param>
        /// <param name="error">Out variable for defining erros.</param>
        /// <returns>Parameter's value of <typeparamref name="T"/> type.</returns>
        T Parse(string input, out Error error);
    }
}
