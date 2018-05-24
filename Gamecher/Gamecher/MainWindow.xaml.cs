using Gamecher.Objects;
using Microsoft.Win32;
using Newtonsoft.Json;
using SharpConfig;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Gamecher
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        public readonly string STEAM_API_KEY = "D297C7CEC2B377B7D4ED0FE086825E28";
        public WrapPanel wrapPanel { get; set; }
        public StackPanel stackPanel { get; set; }


        public MainWindow()
        {
            InitializeComponent();
            wrapPanel = wrapMahepanel;
            stackPanel = StackMahePanel;
        }

        /// <summary>
        /// TitleBar_MouseDown - Drag if single-click, resize if double-click
        /// </summary>
        private void WindowTopBarClicked(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                if (e.ClickCount == 2)
                {
                    AdjustWindowSize();
                }
                else
                {
                    Application.Current.MainWindow.DragMove();
                }


            


        }

        /// <summary>
        /// CloseButton_Clicked
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// MaximizedButton_Clicked
        /// </summary>
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            AdjustWindowSize();
        }

        /// <summary>
        /// Minimized Button_Clicked
        /// </summary>
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Adjusts the WindowSize to correct parameters when Maximize button is clicked
        /// </summary>
        private void AdjustWindowSize()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.ResizeMode = ResizeMode.CanResize;
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.MaxHeight = SystemParameters.WorkArea.Height;
                this.ResizeMode = ResizeMode.NoResize;
                this.WindowState = WindowState.Maximized;
            }

        }

        private void UserAvatarClick(object sender, MouseButtonEventArgs e)
        {
            this.Opacity = 0.9;
            this.Effect = new BlurEffect();

            var userSettings = new UserSettings()
            {
                Owner = this,
                ShowInTaskbar = false
            };

            userSettings.ShowDialog();
        }

        private void AddGameClicked(object sender, MouseButtonEventArgs e)
        {
            /*GameCard gC = new GameCard();
            gC.PlayButton.MouseUp += AddGameClicked;
            wrapMahepanel.Children.Add(gC);*/
            
            this.Opacity = 0.9;
            this.Effect = new BlurEffect();

            var gameAdder = new GameAdder()
            {
                Owner = this,
                ShowInTaskbar = false
            };

            gameAdder.ShowDialog();

        }

        private void GameSettingsClicked(object sender, MouseButtonEventArgs e)
        {
            this.Opacity = 0.9;
            this.Effect = new BlurEffect();

            var configGame = new ConfigGame()
            {
                Owner = this,
                ShowInTaskbar = false
            };

            configGame.ShowDialog();
        }
    }
}
