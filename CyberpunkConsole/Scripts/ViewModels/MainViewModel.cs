using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Windows;

namespace CyberpunkConsole.Scripts.ViewModels
{
    public class MainViewModel: VMBase
    {
        #region Properties

        #region ResizeMode
        private ResizeMode resizeMode = ResizeMode.CanResizeWithGrip;
        public ResizeMode ResizeMode
        {
            get => resizeMode;
            set
            {
                resizeMode = value;
                OnPropertyChanged("ResizeMode");
            }
        }
        #endregion

        #region WindowState

        private WindowState windowState = WindowState.Normal;
        public WindowState WindowState
        {
            get => windowState;
            set
            {
                if (value == WindowState.Normal)
                {
                    WindowBorderThickness = new Thickness(1);
                    ResizeMode = ResizeMode.CanResizeWithGrip;
                }
                windowState = value;
                OnPropertyChanged("WindowState");
            }
        }

        #endregion

        #region WindowBorderThickness

        private Thickness windowBorderThickness = new Thickness(1);
        public Thickness WindowBorderThickness
        {
            get => windowBorderThickness;
            set
            {
                windowBorderThickness = value;
                OnPropertyChanged("WindowBorderThickness");
            }
        }

        #endregion

        #region GridMargin

        private Thickness gridMargin = new Thickness(10);
        public Thickness GridMargin
        {
            get => gridMargin;
            set
            {
                gridMargin = value;
                OnPropertyChanged("GridMargin");
            }
        }

        #endregion

        #endregion

        #region Commands

        #region CloseCommand
        /// <summary>
        /// Action that should close main window.
        /// </summary>
        public Action CloseAction { get; set; }
        private RelayCommand closeCommand;
        /// <summary>
        /// Close window and save decks.
        /// </summary>
        public RelayCommand CloseCommand
        {
            get => closeCommand ?? (closeCommand = new RelayCommand(obj =>
            {

                CloseAction();
            }));

        }
        #endregion

        #region MaximizeCommand

        private RelayCommand maximizeCommand;
        public RelayCommand MaximizeCommand
        {
            get => maximizeCommand ?? (maximizeCommand = new RelayCommand(obj =>
            {
                WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
                if (WindowState == WindowState.Maximized)
                {
                    ResizeMode = ResizeMode.NoResize;
                    WindowBorderThickness = new Thickness(0);
                    GridMargin = new Thickness(7);
                }
                else
                {
                    ResizeMode = ResizeMode.CanResizeWithGrip;
                    WindowBorderThickness = new Thickness(1);
                    GridMargin = new Thickness(10);
                }
            }));
        }
        #endregion

        #region MinimizeCommand
        private RelayCommand minimizeCommand;
        public RelayCommand MinimizeCommand
        {
            get => minimizeCommand ?? (minimizeCommand = new RelayCommand(obj =>
            {
                WindowState = WindowState.Minimized;
            }));
        }
        #endregion

        #region NavigateCommands
        private string lastPage { get; set; }

        /// <summary>
        /// Naviage to page with <paramref name="url"/>
        /// </summary>
        public void Navigate(string url)
        {
            if (url != lastPage)
            {
                Messenger.Default.Send<NavigateArgs>(new NavigateArgs(url));
                lastPage = url;
            }
        }

        private RelayCommand pageNavigateCommand;
        public RelayCommand PageNavigateCommand
        {
            get
            {
                return pageNavigateCommand ?? (pageNavigateCommand = new RelayCommand(obj =>
                {
                    Navigate(obj.ToString());
                }));
            }
        }
        #endregion

        #endregion
    }
}
