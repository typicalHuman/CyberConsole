namespace CyberpunkConsole.Scripts.ViewModels
{
    /// <summary>
    /// Class for naviagation args.
    /// </summary>
    public class NavigateArgs
    {
        /// <summary>
        /// Initializing <paramref name="url"/>.
        /// </summary>
        /// <param name="url">Url of page.</param>
        public NavigateArgs(string url)
        {
            Url = url;
        }

        /// <summary>
        /// Page url.
        /// </summary>
        public string Url { get; set; }
    }
}
