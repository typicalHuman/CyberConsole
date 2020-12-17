using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CyberpunkConsoleControl
{
    /// <summary>
    /// Class for inserting $ symbol in each line. 
    /// </summary>
    public class NewLineMargin: LineNumberMargin
    {
        #region Constants

        private const string DOLLAR_SYMBOL = " $ ";
        private const string EMPTY_SYMBOL = " ";
        /// <summary>
        /// X coordinate for drawing symbol in <see cref="DrawingContext"/>.
        /// </summary>
        private const int INSERTION_X_COORDINATE = -5;

        /// <summary>
        /// All constants underneath are equivalents of <see cref="LineState"/> states.
        /// </summary>

        private const int EMPTY_STATE = (int)LineState.EMPTY_STATE;
        private const int COMMAND_STATE = (int)LineState.COMMAND_STATE;
        private const int EDIT_STATE = (int)LineState.EDIT_STATE;

        #endregion

        #region Properties

        #region Public

        /// <summary>
        /// Symbol which indicates <see cref="ConsoleMode.COMMAND_MODE"/>.
        /// </summary>
        public string EnterSymbol { get; set; } = DOLLAR_SYMBOL;

        #endregion

        #region Private

        /// <summary>
        /// Digital value of <see cref="LineState.EMPTY_STATE"/> state.
        /// </summary>
        private static int REMOVE_SYMBOL { get; set; } = (int)LineState.EMPTY_STATE;

        /// <summary>
        /// Digital value of <see cref="LineState.COMMAND_STATE"/> state.
        /// </summary>
        private static int INSERT_SYMBOL { get; set; } = (int)LineState.COMMAND_STATE;

        /// <summary>
        /// List to remove excess symbols from left margin field.
        /// </summary>
        private List<int> LeftMarginStatesList { get; set; } = new List<int>() { INSERT_SYMBOL };


        #endregion



        #endregion

        #region Fields

        private delegate void SetNextState();

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Update <see cref="INSERT_SYMBOL"/> after ConsoleMode change.
        /// </summary>
        /// <param name="mode">Current console mode.</param>
        public void UpdateLineStates(ConsoleMode mode)
        {
            if (mode == ConsoleMode.COMMAND_MODE)
                INSERT_SYMBOL = (int)LineState.COMMAND_STATE;
            else
                INSERT_SYMBOL = (int)LineState.EDIT_STATE;
            while (TextView.Document.LineCount < LeftMarginStatesList.Count)
                LeftMarginStatesList.RemoveAt(LeftMarginStatesList.Count - 1);
        }

        /// <summary>
        /// Set <see cref="REMOVE_SYMBOL"/> value to element with <paramref name="index"/> index.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveLine(int index)
        {
            LeftMarginStatesList[index] = REMOVE_SYMBOL;
        }

        /// <summary>
        /// Add element in <see cref="LeftMarginStatesList"/> with <see cref="INSERT_SYMBOL"/> value. 
        /// </summary>
        public void AddLine()
        {
            LeftMarginStatesList.Add(INSERT_SYMBOL);
        }

        /// <summary>
        /// Set <see cref="REMOVE_SYMBOL"/> value for range of elements.
        /// </summary>
        /// <param name="start">Range start index.</param>
        /// <param name="end">Range end index.</param>
        public void RemoveLines(int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                if (i < LeftMarginStatesList.Count)
                    RemoveLine(i);
                else
                    RemoveNextState();
            }
            if(INSERT_SYMBOL == COMMAND_STATE)
                 LeftMarginStatesList.Add(INSERT_SYMBOL);
        }

        /// <summary>
        /// Add element in <see cref="LeftMarginStatesList"/> with <see cref="REMOVE_SYMBOL"/> value.
        /// </summary>
        public void RemoveNextState()
        {
            LeftMarginStatesList.Add(REMOVE_SYMBOL);
        }

        /// <summary>
        /// Create <see cref="NewLineMargin"/> instance.
        /// </summary>
        /// <returns><see cref="NewLineMargin"/> instance.</returns>
        public static NewLineMargin Create()
        {
            return new NewLineMargin();
        }

        #endregion

        #region Overrided

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (TextView != null && TextView.VisualLinesValid && TextView.VisualLines.Count > 0)
            {
                SetNextState setNextState = GetNextStateMethod();
                Brush foreground = (Brush)GetValue(Control.ForegroundProperty);
                while (TextView.VisualLines[TextView.VisualLines.Count - 1].FirstDocumentLine.LineNumber > LeftMarginStatesList.Count)
                    setNextState?.Invoke();
                if(setNextState == RemoveNextState)
                    LeftMarginStatesList[LeftMarginStatesList.Count - 1] = INSERT_SYMBOL;//for setting command symbol after mode change
                DrawText(drawingContext, foreground);
            }
        }
        #endregion

        #region Private

        private SetNextState GetNextStateMethod()
        {
            if (INSERT_SYMBOL == COMMAND_STATE)
                return RemoveNextState;
            return AddLine;
        }

        private void DrawText(DrawingContext drawingContext, Brush foreground)
        {
            for (int i = 0; i < TextView.VisualLines.Count; i++)
            {
                VisualLine line = TextView.VisualLines[i];
                FormattedText text = GetTextByState(LeftMarginStatesList[line.FirstDocumentLine.LineNumber - 1], foreground);
                double y = line.GetTextLineVisualYPosition(line.TextLines[0], VisualYPosition.TextTop);
                drawingContext.DrawText(text, new Point(INSERTION_X_COORDINATE, y - TextView.VerticalOffset));
            }
        }

        /// <summary>
        /// Get text by digital value of state.
        /// </summary>
        /// <param name="marginState">Digital value of state.</param>
        /// <param name="foreground">Text foreground.</param>
        /// <returns>Text to draw.</returns>
        private FormattedText GetTextByState(int marginState, Brush foreground)
        {
            if (marginState == REMOVE_SYMBOL)
                return new FormattedText(EMPTY_SYMBOL, CultureInfo.CurrentCulture,
                                                       FlowDirection.LeftToRight,
                                                       typeface, emSize, foreground);
            else if (marginState == EDIT_STATE)
                return new FormattedText(EnterSymbol, CultureInfo.CurrentCulture,
                                                                           FlowDirection.LeftToRight,
                                                                           typeface, emSize, foreground);
            return new FormattedText(DOLLAR_SYMBOL, CultureInfo.CurrentCulture,
                                                                        FlowDirection.LeftToRight,
                                                                        typeface, emSize, foreground);
        }

        #endregion

        #endregion  

    }
}