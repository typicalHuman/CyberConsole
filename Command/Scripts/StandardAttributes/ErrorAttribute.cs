using Command.Errors;
using Command.Interfaces;

namespace Command.StandardAttributes
{
    /// <summary>
    /// Attribute for setting error in attribute definition.
    /// </summary>
    internal class ErrorAttribute: IAttrib
    {
        public void Action(object[] args = null)
        {
        }

        public string Value { get; set; } = null;

        public bool Equals(string parameter) => false;

        public string Message { get; private set; }

        public Error Error { get; set; }
        public int Offset { get; set; }
        public int EndOffset { get; set; }

        public virtual string Description { get; protected set; } = @"Oops ¯\_(ツ)_/¯ Error attribute!";
    }
}
