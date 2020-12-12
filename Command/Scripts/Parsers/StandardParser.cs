using Command.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Command.Parsers
{
    /// <summary>
    /// Standard behavioral parser.
    /// </summary>
    public class StandardParser : IParser
    {
        #region Constants

        /// <summary>
        /// To split parameters by spaces and quotes.
        /// </summary>
        private const string PARSE_PARAMETERS_PATTERN = @"[\""].+?[\""]|[^ ]+";

        #endregion

        #region Properties

        /// <summary>
        /// Command splited by spaces.
        /// </summary>
        private string[] splitedCommand { get; set; }

        #endregion

        #region Methods

        #region Public

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
                SetSplitedCommandValue(commandLineText);
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
                IEnumerable<string> attributeSpellings = GetAttributeSpellings(commandLineText);
                return GetAttributes(command.StandardAttributes, attributeSpellings).ToArray();
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
        private void SetSplitedCommandValue(string commandLineText)
        {
            splitedCommand = Regex.Matches(commandLineText, PARSE_PARAMETERS_PATTERN)
                .Cast<Match>()
                .Select(m => m.Value)
                .ToArray();
        }

        /// <summary>
        /// Get command root in the <paramref name="commandLineText"/>.
        /// </summary>
        /// <param name="commandLineText"></param>
        /// <returns>String value of command identifier.</returns>
        private string GetCommandSpelling(string commandLineText)
        {
            SetSplitedCommandValue(commandLineText);
            return splitedCommand[0];
        }

        /// <summary>
        /// Get attributes string values from <paramref name="commandLineText"/>.
        /// </summary>
        /// <param name="commandLineText">Line to parse.</param>
        /// <returns>Enumerable of attributes string values.</returns>
        private IEnumerable<string> GetAttributeSpellings(string commandLineText)
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
        private IEnumerable<IAttrib> GetAttributes(IAttrib[] standardAttributes, IEnumerable<string> attributeSpellings)
        {
            foreach(string spelling in attributeSpellings)
            {
                foreach (IAttrib attrib in standardAttributes)
                    if (attrib.Equals(spelling))
                        yield return attrib;
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
            List<string> attributes = GetAttributeSpellings(commandLineText).ToList();
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
        #endregion

        #endregion

    }
}
