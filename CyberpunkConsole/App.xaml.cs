using CyberpunkConsole.Scripts.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CyberpunkConsole
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MainViewModel MainVM { get; set; }
        public static ConsoleViewModel ConsoleVM { get; set; }
        static App()
        {
            MainVM = new MainViewModel();
            ConsoleVM = new ConsoleViewModel();
        }
    }
}
