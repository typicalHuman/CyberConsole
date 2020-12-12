using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CyberpunkConsole.Scripts.ViewModels
{
    /// <summary>
    /// Base for all view models. It includes property changed methods <see cref="PropertyChanged"/>.
    /// </summary>
    public class VMBase: INotifyPropertyChanged
    {
        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion
    }
}
