using CyberpunkConsoleControl;

namespace Commands.StandardCommands
{
    internal static class AsyncExtension
    {
        /// <summary>
        /// Method for inserting text asynchronously.
        /// </summary>
        /// <param name="cc">CyberConsole parameter (for extension work).</param>
        /// <param name="text">Text to insert.</param>
        /// <param name="start">Start line. (if you have inserted text before async func).</param>
        public static void InsertTextAsync(this CyberConsole cc, string text, int start = -1)
        {
            cc.Dispatcher.Invoke(() =>
            {
                if (start == -1)
                    start = cc.Document.LineCount - 1;
                cc.InsertText(text, true);
                int end = cc.Document.LineCount - 1;
                (cc.TextArea.LeftMargins[0] as NewLineMargin).RemoveLines(start, end);
                cc.IsEnabled = true;
                cc.Focus();
            });
        }
    }
}
