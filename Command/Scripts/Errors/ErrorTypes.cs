﻿namespace Command.Errors
{
    /// <summary>
    /// Input error types.
    /// </summary>
    public enum ErrorTypes
    {
        NONE = 0,
        ANOTHER_PARAMETER,
        SYNTAX_ERROR,
        UNCLOSED_QUOTE,
        OUT_OF_RANGE_NUMBER,
        WRONG_INPUT,
        NULL_VALUE,
        NOT_FOUND,
        WRONG_PARAMETERS_COUNT
    }
}
