using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Gamecher
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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

            DirectoryInfo dir_info = new DirectoryInfo("C:\\");
            List<string> dir_list = new List<string>();
            SearchDirectory(dir_info, dir_list);

            foreach (string i in dir_list)
            {
                if (i.Contains("coprocmanager"))
                {
                    string awidnawod = new Regex(@"\bcoprocmanager\b").Split(i)[0] + "coprocmanager";
                    Console.WriteLine(awidnawod);
                    break;
                }
            };

            dir_info = new DirectoryInfo("D:\\");
            dir_list = new List<string>();
            SearchDirectory(dir_info, dir_list);

            foreach (string i in dir_list)
            {
                if (i.Contains("steamapps"))
                {
                    string awidnawod = new Regex(@"\bsteamapps\b").Split(i)[0] + "steamapps";
                    Console.WriteLine(awidnawod);
                    break;
                }
            };

            string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        if (subkey.GetValue("DisplayName") != null && subkey.GetValue("InstallLocation") != null)
                        {
                            if (!subkey.GetValue("InstallLocation").ToString().Equals(""))
                            {
                                Console.WriteLine("DisplayName: " + subkey.GetValue("DisplayName"));
                                Console.WriteLine("InstallLocation: " + subkey.GetValue("InstallLocation"));
                                List<String> exeList = GetFiles(subkey.GetValue("InstallLocation").ToString(), ".exe", SearchOption.AllDirectories).Cast<String>().ToList();
                                exeList.ForEach(i => Console.WriteLine(i));

                                Console.WriteLine();
                            }
                        }
                    }
                }
            }

            //List<String> prueba = Search("D:/", ".txt", SearchOption.AllDirectories);
            //prueba.ForEach(i => Console.WriteLine(i));
            

        }

        public static IEnumerable<string> GetFiles(string path,
                       string searchPatternExpression = "",
                       SearchOption searchOption = SearchOption.AllDirectories)
        {
            Regex reSearchPattern = new Regex(searchPatternExpression, RegexOptions.IgnoreCase);
            return Directory.EnumerateFiles(path, "*", searchOption)
                            .Where(file =>
                                     reSearchPattern.IsMatch(Path.GetExtension(file)));
        }

        private void SearchDirectory(DirectoryInfo dir_info, List<string> dir_list)
        {
            try
            {
                foreach (DirectoryInfo subdir_info in dir_info.GetDirectories())
                {
                    
                    SearchDirectory(subdir_info, dir_list);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            try
            {
                foreach (DirectoryInfo subsubdir_info in dir_info.GetDirectories())
                {
                    dir_list.Add(dir_info.FullName);
                }
            }
            catch
            {
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
            var userSettings = new UserSettings();
            userSettings.ShowDialog();
        }

        private void AddGameClicked(object sender, MouseButtonEventArgs e)
        {
            GameCard gC = new GameCard();
            gC.PlayButton.MouseUp += AddGameClicked;
            wrapMahepanel.Children.Add(gC);
        }
    }
}
