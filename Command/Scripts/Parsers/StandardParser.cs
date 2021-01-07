using Command.StandardAttributes;
using Command.Errors;
using Command.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Command.Parsers
{
    /// <summary>
    /// Standard behavioural parser.
    /// </summary>
    public class StandardParser : IParser
    {
        #region Constructors

        /// <summary>
        /// Standard constructor.
        /// </summary>
        public StandardParser()
        {

        }

        /// <summary>
        /// Initialize regex pattern.
        /// </summary>
        /// <param name="parsePattern">Pattern to parse parameters of command.</param>
        public StandardParser(string parsePattern)
        {
            if(!string.IsNullOrEmpty(parsePattern))
                PARSE_PARAMETERS_PATTERN = parsePattern;
        }

        #endregion

        #region Constants

        /// <summary>
        /// To split parameters by spaces and quotes.
        /// </summary>
        private readonly string PARSE_PARAMETERS_PATTERN = "\"([^']*)\"|[^ ]+";

        /// <summary>
        /// <see cref="string.IndexOf(string)"/>'s not found value.
        /// </summary>
        private const int VALUE_NOT_FOUND = -1;

        #endregion

        #region Properties

        /// <summary>
        /// Command splited by spaces.
        /// </summary>
        private string[] splitedCommand { get; set; }

        #endregion

        #region Methods

        #region Public

        public static void SetOffset(IOffset offset, string value, ref string lineToParse, ref int lengthToSum)
        {
            int index = lineToParse.IndexOf(value);
            if (index != VALUE_NOT_FOUND)
            {
                offset.Offset = index + lengthToSum;
                offset.EndOffset = offset.Offset + value.Length;
                lineToParse = lineToParse.Remove(offset.Offset - lengthToSum, value.Length);
                lengthToSum += value.Length;
            }
        }

        public static void SetOffset(IParameter param, ref string lineToParse, ref int lengthToSum)
        {
            SetOffset(param, param.Value, ref lineToParse, ref lengthToSum);
        }

        /// <summary>
        /// Is <paramref name="commandLineText"/> suited for command lexic.
        /// </summary>
        /// <param name="spelling">Command spelling.</param>
        /// <param name="commandLineText">Line to parse.</param>
        /// <returns>Is input suited for command lexic with <paramref name="spelling"/> spelling.</returns>
        public bool IsCommandLexic(string spelling, string commandLineText)
        {
            return spelling == GetCommandSpelling(commandLineText);
        }

        /// <summary>
        /// Is <paramref name="commandLineText"/> attributes suited for command standar attributes lexic.
        /// </summary>
        /// <param name="standardAttributes">Array of avaliable attributes.</param>
        /// <param name="commandLineText">Line to parse.</param>
        /// <returns>Is input attributes suited for <paramref name="standardAttributes"/> of command.</returns>
        public bool IsAttributesLexic(IAttrib[] standardAttributes, string commandLineText)
        {
            if (splitedCommand == null)
                splitedCommand = GetSplitedCommandValue(commandLineText);
            return IsAttributesLexic(standardAttributes);
        }

        /// <summary>
        /// Get avaliable attributes of <paramref name="command"/>.
        /// </summary>
        /// <param name="command">Command which attributes will parse in the <paramref name="commandLineText"/>.</param>
        /// <param name="commandLineText">Line to parse.</param>
        /// <returns>Array of found attributes or empty array. </returns>
        public IAttrib[] GetAttributes(ICommand command, string commandLineText)
        {
            if (IsCommandLexic(command.Spelling, commandLineText))
            {
                IEnumerable<string> attributeSpellings = GetAllParametersSpellings(commandLineText);
                return GetAttributes(command.StandardAttributes, attributeSpellings, commandLineText).ToArray();
            }
            else
                return new IAttrib[] { };
        }


        #endregion

        #region Private

        /// <summary>
        /// Splite command by spaces. (first will be command spelling, the rest are parameters).
        /// </summary>
        /// <param name="commandLineText">Line to parse.</param>
        private string[] GetSplitedCommandValue(string commandLineText)
        {
            string[] splited = Regex.Matches(commandLineText, PARSE_PARAMETERS_PATTERN)
                .Cast<Match>()
                .Select(m => m.Value).ToArray();
            return splited;
        }

        /// <summary>
        /// Get command root in the <paramref name="commandLineText"/>.
        /// </summary>
        /// <param name="commandLineText"></param>
        /// <returns>String value of command identifier.</returns>
        private string GetCommandSpelling(string commandLineText)
        {
            splitedCommand = GetSplitedCommandValue(commandLineText);
            return splitedCommand[0].Replace(" ", "");
        }

        /// <summary>
        /// Get parameters string values from <paramref name="commandLineText"/>.
        /// </summary>
        /// <param name="commandLineText">Line to parse.</param>
        /// <returns>Enumerable of parameters string values.</returns>
        private IEnumerable<string> GetParametersSpellings(string commandLineText)
        {
            for(int i = 1; i < splitedCommand.Length; i++)
                yield return splitedCommand[i];
        }

        /// <summary>
        /// Is standard attributes contain specified attributes.
        /// </summary>
        /// <param name="standardAttrubutes">Command avaliable attributes. </param>
        /// <returns>Is attributes in <see cref=" splitedCommand"/> array suited for <paramref name="standardAttrubutes"/> lexic.</returns>
        private bool IsAttributesLexic(IAttrib[] standardAttrubutes)
        {
            bool isLexic;
            for (int i = 1; i < splitedCommand.Length; i++)
            {
                isLexic = false;
                foreach (IAttrib attrib in standardAttrubutes)
                {
                    if (attrib.Equals(splitedCommand[i]))
                    {
                        isLexic = true;
                        break;
                    }
                }
                if (!isLexic)
                    return false;
            }
            return false;
        }
        /// <summary>
        /// Get attrubutes by them string values.
        /// </summary>
        /// <param name="standardAttributes">Command avaliable attributes.</param>
        /// <param name="attributeSpellings">Enumerable of attributes string values.</param>
        /// <returns>Enumerable of found attributes.</returns>
        private IEnumerable<IAttrib> GetAttributes(IAttrib[] standardAttributes, IEnumerable<string> attributeSpellings, string commandLineText)
        {
            int lengthToSum = 0;
            foreach(string spelling in attributeSpellings)
            {
                bool isFound = false;
                foreach (IAttrib attrib in standardAttributes)
                {
                    if (attrib.Equals(spelling))
                    {
                        SetOffset(attrib, ref commandLineText, ref lengthToSum);
                        isFound = true;
                        yield return attrib;
                    }
                }
                if (!isFound)
                {
                    ErrorAttribute errAttrib = new ErrorAttribute() { Error = new ParameterNotFoundError(spelling), Value = spelling };
                    SetOffset(errAttrib, ref commandLineText, ref lengthToSum);
                    yield return errAttrib;
                }
            }
        }


        /// <summary>
        /// Get parameters by excluding attributes from <paramref name="commandLineText"/>.
        /// </summary>
        /// <param name="commandLineText">Line to parse.</param>
        /// <param name="standardAttributes">Command avaliable attributes.</param>
        /// <returns>Enumerable of parameters string values.</returns>
        public IEnumerable<string> GetParameters(string commandLineText, IAttrib[] standardAttributes)
        {
            List<string> attributes = GetAllParametersSpellings(commandLineText).ToList();
            bool isContains;
            for (int i = 1; i < splitedCommand.Length; i++)
            {
                isContains = false;
                for (int j = 0; j < attributes.Count && standardAttributes != null; j++)
                {
                    if(standardAttributes.Select(sa => sa.Value).Contains(attributes[j]))
                    {
                        attributes.Remove(attributes[j]);
                        j--;
                        isContains = true;
                        break;
                    }
                }
                if (!isContains)
                    yield return splitedCommand[i];
            }
        }

        /// <summary>
        /// Get attributes string values from <paramref name="commandLineText"/>.
        /// </summary>
        /// <param name="commandLineText">Line to parse.</param>
        /// <returns>Enumerable of attributes string values.</returns>
        private IEnumerable<string> GetAllParametersSpellings(string commandLineText)
        {
            for (int i = 1; i < splitedCommand.Length; i++)
                yield return splitedCommand[i];
        }
        #endregion

        #endregion

    }
}
