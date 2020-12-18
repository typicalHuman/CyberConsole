using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CyberpunkConsoleControl
{
    /// <summary>
    /// Class for CyberConsole control.
    /// </summary>
    public class CyberConsole : TextEditor, INotifyPropertyChanged
    {
        #region Constructors

        public CyberConsole()
        {
            TextSegmentCollection<TextSegment> col = new TextSegmentCollection<TextSegment>();
            TextArea.ReadOnlySectionProvider = new TextSegmentReadOnlySectionProvider<TextSegment>(col);
            TextArea.Caret.PositionChanged += OnCaretPositionChanged;
            TextArea.TextEntering += OnTextEntering;
            TextArea.LeftMargins.Add(NewLineMargin.Create());
            TextArea.SelectionBrush = (Brush)new BrushConverter().ConvertFrom("#4b5e68a1");
            TextArea.SelectionForeground = (Brush)new BrushConverter().ConvertFrom("#579571");
            TextArea.SelectionBorder = new Pen(Brushes.Transparent, 0);
        }

        #endregion

        #region Properties

        #region Public

        #region Text
        /// <summary>
        /// A bindable Text property
        /// </summary>
        public new string Text
        {
            get => (string)GetValue(TextProperty);
            set
            {
                SetValue(TextProperty, value);
                OnPropertyChanged("Text");
            }
        }
        #endregion

        #region EnterSymbol

        public string EnterSymbol
        {
            get => (string)GetValue(EnterSymbolProperty);
            set
            {
                SetValue(EnterSymbolProperty, value);
                OnPropertyChanged("EnterSymbol");
            }
        }
        #endregion

        #region ConsoleMode

        public ConsoleMode ConsoleMode
        {
            get => (ConsoleMode)GetValue(ConsoleModeProperty);
            set
            {
                SetValue(ConsoleModeProperty, value);
                OnPropertyChanged("ConsoleMode");
            }
        }
        #endregion

        #endregion

        #region Private

        /// <summary>
        /// Indicator for defining readonly segments.
        /// </summary>
        private int lastCaretLine { get; set; } = 1;

        /// <summary>
        /// Did console mode change? 
        /// (property is needed for setting readonly segment in static <see cref="OnConsoleModeChanged(DependencyObject, DependencyPropertyChangedEventArgs)"/>.
        /// </summary>
        private bool isConsoleModeChanged { get; set; } = false;

        #endregion

        #endregion

        #region DependencyProperty

        #region TextProperty

        /// <summary>
        /// The bindable text property dependency property
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(CyberConsole),
                new FrameworkPropertyMetadata
                {
                    DefaultValue = default(string),
                    BindsTwoWayByDefault = true,
                    PropertyChangedCallback = OnTextPropertyChanged
                }
            );

        #endregion

        #region EnterSymbolProperty

        public static readonly DependencyProperty EnterSymbolProperty =
            DependencyProperty.Register(
                "EnterSymbol",
                typeof(string),
                typeof(CyberConsole),
                new FrameworkPropertyMetadata
                {
                    DefaultValue = " $ ",
                    BindsTwoWayByDefault = true,
                    PropertyChangedCallback = OnEnterSymbolChanged
                }
            );

        #endregion

        #region ConsoleModeProperty

        public static readonly DependencyProperty ConsoleModeProperty =
            DependencyProperty.Register(
                "ConsoleMode",
                typeof(ConsoleMode),
                typeof(CyberConsole),
                new FrameworkPropertyMetadata
                {
                    DefaultValue = ConsoleMode.COMMAND_MODE,
                    BindsTwoWayByDefault = true,
                    PropertyChangedCallback = OnConsoleModeChanged
                }
            );

        #endregion

        #endregion

        #region Events

        #region DependencyPropertyChanged

        #region OnTextPropertyChanged

        protected static void OnTextPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CyberConsole target = (CyberConsole)obj;

            if (target.Document != null)
            {
                int caretOffset = target.CaretOffset;
                object newValue = args.NewValue;



                if (newValue == null)
                {
                    newValue = "";
                }

                target.Document.Text = (string)newValue;
            }
        }
        #endregion

        #region OnEnterSymbolChanged

        protected static void OnEnterSymbolChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CyberConsole target = (CyberConsole)obj;
            (target.TextArea.LeftMargins[0] as NewLineMargin).EnterSymbol = (string)args.NewValue;
        }

        #endregion

        #region OnConsoleModeChanged

        protected static void OnConsoleModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CyberConsole target = (CyberConsole)obj;
            (target.TextArea.LeftMargins[0] as NewLineMargin).UpdateLineStates((ConsoleMode)args.NewValue);
            target.isConsoleModeChanged = true;
        }

        #endregion

        #endregion

        #region Private

        #region CaretPositionChanged
        /// <summary>
        /// Setting last caret line for excluding writing on previous lines.         
        /// </summary>
        private void OnCaretPositionChanged(object sender, EventArgs e)
        {
            while (TextArea.Caret.Line > lastCaretLine)
            {
                if (ConsoleMode == ConsoleMode.COMMAND_MODE)
                    CreatePreviousLineReadonlySegment();
                lastCaretLine++;
            }
            if (Document.LineCount < lastCaretLine)
                lastCaretLine = Document.LineCount;
            else
                lastCaretLine = TextArea.Caret.Line;
        }

        #endregion

        #endregion

        #region Overrided

        #region OnTextChanged

        protected override void OnTextChanged(EventArgs e)
        {
            if (this.Document != null)
            {
                var a = CaretOffset;
                Text = this.Document.Text;
                TextArea.Caret.Offset = a;
            }

            base.OnTextChanged(e);
        }


        #endregion

        #region OnTextEntering

        /// <summary>
        /// Event for check input before <see cref="OnTextChanged(EventArgs)"/>.
        /// </summary>
        private void OnTextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "\n")
            {
                TextArea.Caret.Offset = Document.Lines.Last().EndOffset;
            }
            if (IsPriorLine() && ConsoleMode == ConsoleMode.COMMAND_MODE)
                e.Handled = true;
        }

        #endregion

        #region OnPreviewKeyDown

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (IsRemovingTextWithAnotherSelection())
            {
                if (ConsoleMode == ConsoleMode.COMMAND_MODE)
                    e.Handled = true;
                TextArea.Caret.Line = lastCaretLine;//setting caret on last line (to remove cases where the caret stays on the previous lines that are readonly)
            }
            base.OnPreviewKeyDown(e);
        }

        #endregion

        #region OnKeyUp

        protected override void OnKeyUp(KeyEventArgs e)
        {
            //if new line
            if (e.Key == Key.Enter)
                ProcessCommand();
            base.OnKeyDown(e);
        }

        #endregion

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion

        #endregion


        #endregion

        #region Methods

        #region Private

        /// <summary>
        /// Process command after enter input.
        /// </summary>
        private void ProcessCommand()
        {
            string commandLineText = GetCurrentLineText();
            if (ConsoleMode == ConsoleMode.COMMAND_MODE)
            {
                ExecuteCommand(commandLineText);
            }
            if (isConsoleModeChanged)
            {
                UpdateConsoleModeChanged();
            }
            if (ConsoleMode == ConsoleMode.EDITOR_MODE)
            {
                (TextArea.LeftMargins[0] as NewLineMargin).AddLine();
            }
        }




        /// <summary>
        /// Execute command by <paramref name="commandLineText"/> input.
        /// </summary>
        /// <param name="commandLineText">Command text to parse.</param>
        private void ExecuteCommand(string commandLineText)
        {
            int start = lastCaretLine - 1;
            CommandsManager.ExecuteCommand(commandLineText, this);
            int end = lastCaretLine - 1;
            ScrollToEnd();
            (TextArea.LeftMargins[0] as NewLineMargin).RemoveLines(start, end);
        }

        /// <summary>
        /// Update last readonly segment 
        /// (for excluding error with <see cref="OnCaretPositionChanged(object, EventArgs)"/>).
        /// </summary>
        private void UpdateConsoleModeChanged()
        {
            CreatePreviousLineReadonlySegment();
            isConsoleModeChanged = false;
        }

        /// <summary>
        /// Is the current line preceded by a previous line. 
        /// </summary>
        private bool IsPriorLine()
        {
            return Document.LineCount > lastCaretLine;
        }

        /// <summary>
        /// Set text on previous line readonly.
        /// </summary>
        private void CreatePreviousLineReadonlySegment()
        {
            TextSegment seg = new TextSegment();
            TextSegmentCollection<TextSegment> segments = (TextArea.ReadOnlySectionProvider as TextSegmentReadOnlySectionProvider<TextSegment>).Segments;
            int lineNum = 0;
            if (segments.Count > 0)
                  lineNum = Document.GetLineByOffset(segments.Last().StartOffset + 1).LineNumber;
            //penultimate line
            if (lineNum < Document.LineCount - 1)
            {
                var line = Document.Lines[lineNum];
                seg.StartOffset = line.Offset;
                if (seg.StartOffset > 0)
                    seg.StartOffset--;
                seg.EndOffset = line.EndOffset + 1;//to remove cases when with removing line first letters goes to previous line (which is readonly)
                if (!segments.Contains(seg))
                    segments.Add(seg);
            }
        }


        /// <summary>
        /// Is text removing with selection on current and another lines.
        /// </summary>
        private bool IsRemovingTextWithAnotherSelection()
        {
            int startLineNum = TextArea.Selection.StartPosition.Line;
            int lastLineNum = Document.LineCount;
            if (startLineNum != 0)
                return startLineNum != lastLineNum;
            return false;
        }

        /// <summary>
        /// Get text of line with <see cref="lastCaretLine"/> number.
        /// </summary>
        /// <returns>Line's text.</returns>
        private string GetCurrentLineText()
        {
            DocumentLine line = Document.GetLineByNumber(lastCaretLine - 1);
            return Document.GetText(line.Offset, line.Length);
        }

        #endregion

        #region Public

        /// <summary>
        /// Select text on current row.
        /// </summary>
        public void SelectRow()
        {
            DocumentLine line = Document.Lines[TextArea.Caret.Line - 1];
            Select(line.Offset, line.Length);
        }

        /// <summary>
        /// Insert <paramref name="textToInsert"/> in Console.
        /// </summary>
        /// <param name="textToInsert">Text which will be inserted.</param>
        /// <param name="isNewLine">Insert with addition of new line.</param>
        internal void InsertText(string textToInsert, bool isNewLine = false)
        {
            Text = Text.Insert(Text.Length, $"{textToInsert}");
            if (isNewLine)
                Text = Text.Insert(Text.Length, "\n");
        }


        #endregion

        #endregion
    }
}
