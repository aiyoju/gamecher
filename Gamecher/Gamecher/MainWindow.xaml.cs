using Gamecher.Objects;
using Microsoft.Win32;
using Newtonsoft.Json;
using SharpConfig;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;

namespace Gamecher
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        public readonly string STEAM_API_KEY = "D297C7CEC2B377B7D4ED0FE086825E28";

        public MainWindow()
        {
            InitializeComponent();
            GameCard gC = new GameCard();
            //gC.PlayButton.MouseUp += PlayButtonClicked;
            gC.GameSettings.MouseUp += GameSettingsClicked;
            wrapMahepanel.Children.Add(gC);
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


            searchForSteamGames();


        }

        public void searchForSteamGames()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            List<string> pathsFound = new List<string>();
            List<string> allAppidFiles = new List<string>();
            List<string> allAppids = new List<string>();
            int numberOfTask = 0;
            foreach (DriveInfo drive in drives)
            {

                Task t = Task.Factory.StartNew(() =>
                {

                    DirectoryInfo dir_info = new DirectoryInfo(drive.Name);
                    List<string> dir_list = new List<string>();
                    SearchDirectories(dir_info, dir_list);

                    List<string> resultPaths = new List<string>();

                    foreach (string i in dir_list)
                    {
                        if (i.Contains("steamapps"))
                        {
                            if (new Regex(@"steamapps").Split(i)[1].Equals(""))
                            {
                                string resultPath = new Regex(@"steamapps").Split(i)[0] + "steamapps";
                                if (!resultPaths.Contains(resultPath))
                                {
                                    resultPaths.Add(resultPath);
                                    pathsFound.Add(resultPath);
                                }
                            }
                        }
                    }

                    foreach (string resultPath in resultPaths)
                    {
                        if (!resultPath.Equals(""))
                        {
                            List<string> allappidfilesCache = GetFiles(resultPath, ".acf", SearchOption.TopDirectoryOnly).Cast<String>().ToList();
                            List<string> allappidfiles = new List<string>();

                            allappidfilesCache.ForEach(i => allAppidFiles.Add(i));
                            foreach (string i in allappidfilesCache)
                            {
                                string appid = Regex.Split(i, @"appmanifest_")[1];
                                appid = Regex.Split(appid, @".acf")[0];
                                allappidfiles.Add(appid);

                                string json = @"{""app";
                                string getJson = HTTPUtils.HTTPGet("https://store.steampowered.com/api/appdetails", "?appids=" + appid + "&l=english");
                                Console.WriteLine(getJson);
                                var jsonArray = Regex.Split(getJson, @"""");
                                for (int j = 2; j < jsonArray.Length; j++)
                                {
                                    json += @"""" + jsonArray[j];
                                }

                                var steamGame = JsonConvert.DeserializeObject<RootObject>(json);
                                Console.WriteLine(steamGame.app.data.name);
                                Console.WriteLine(steamGame.app.data.about_the_game);
                                if (steamGame.app.data.metacritic != null) Console.WriteLine(steamGame.app.data.metacritic.score);
                                steamGame.app.data.publishers.ForEach(e => Console.WriteLine(e));
                                Console.WriteLine();
                                steamGame.app.data.categories.ForEach(e => Console.WriteLine(e.description));
                                Console.WriteLine();
                                steamGame.app.data.genres.ForEach(e => Console.WriteLine(e.description));
                                Console.WriteLine();
                            }

                            allappidfiles.ForEach(i => allAppids.Add(i));
                        }
                    }
                }).ContinueWith(tsk =>
                {
                    numberOfTask++;
                    if (numberOfTask == drives.Length)
                    {
                        List<Juego> games = new List<Juego>();

                        pathsFound.ForEach(i => Console.WriteLine(i));
                        allAppidFiles.ForEach(i => Console.WriteLine(i));
                        allAppids.ForEach(i => games.Add(new Juego() { appid = i }));

                        /* games.Add(pathsFound);
                         games.Add(allAppids);

                         returnSteamGames(games);*/
                    }
                });
            }
        }

        public List<Juego> returnSteamGames(List<Juego> games)
        {
            return games;
        }

        public void searchForBattlenetGames()
        {
            Task t = Task.Factory.StartNew(() =>
            {
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
                                    List<String> exeList = null;
                                    switch (subkey.GetValue("DisplayName").ToString().ToLower())
                                    {
                                        case "overwatch":
                                            Console.WriteLine("DisplayName: " + subkey.GetValue("DisplayName"));
                                            Console.WriteLine("InstallLocation: " + subkey.GetValue("InstallLocation"));
                                            exeList = GetFiles(subkey.GetValue("InstallLocation").ToString(), ".exe", SearchOption.AllDirectories).Cast<String>().ToList();
                                            exeList.ForEach(i => Console.WriteLine(i));

                                            //Process.Start(@"battlenet://Pro");

                                            Console.WriteLine();
                                            break;
                                        case "hearthstone":
                                            Console.WriteLine("DisplayName: " + subkey.GetValue("DisplayName"));
                                            Console.WriteLine("InstallLocation: " + subkey.GetValue("InstallLocation"));
                                            exeList = GetFiles(subkey.GetValue("InstallLocation").ToString(), ".exe", SearchOption.AllDirectories).Cast<String>().ToList();
                                            exeList.ForEach(i => Console.WriteLine(i));

                                            Console.WriteLine();
                                            break;
                                        case "heroes of the storm":
                                            Console.WriteLine("DisplayName: " + subkey.GetValue("DisplayName"));
                                            Console.WriteLine("InstallLocation: " + subkey.GetValue("InstallLocation"));
                                            exeList = GetFiles(subkey.GetValue("InstallLocation").ToString(), ".exe", SearchOption.AllDirectories).Cast<String>().ToList();
                                            exeList.ForEach(i => Console.WriteLine(i));

                                            Console.WriteLine();
                                            break;

                                        case "diablo iii":
                                            Console.WriteLine("DisplayName: " + subkey.GetValue("DisplayName"));
                                            Console.WriteLine("InstallLocation: " + subkey.GetValue("InstallLocation"));
                                            exeList = GetFiles(subkey.GetValue("InstallLocation").ToString(), ".exe", SearchOption.AllDirectories).Cast<String>().ToList();
                                            exeList.ForEach(i => Console.WriteLine(i));

                                            Console.WriteLine();
                                            break;
                                        case "diablo ii":
                                            Console.WriteLine("DisplayName: " + subkey.GetValue("DisplayName"));
                                            Console.WriteLine("InstallLocation: " + subkey.GetValue("InstallLocation"));
                                            exeList = GetFiles(subkey.GetValue("InstallLocation").ToString(), ".exe", SearchOption.AllDirectories).Cast<String>().ToList();
                                            exeList.ForEach(i => Console.WriteLine(i));

                                            Console.WriteLine();
                                            break;
                                        case "world of warcraft":
                                            Console.WriteLine("DisplayName: " + subkey.GetValue("DisplayName"));
                                            Console.WriteLine("InstallLocation: " + subkey.GetValue("InstallLocation"));
                                            exeList = GetFiles(subkey.GetValue("InstallLocation").ToString(), ".exe", SearchOption.AllDirectories).Cast<String>().ToList();
                                            exeList.ForEach(i => Console.WriteLine(i));

                                            Console.WriteLine();
                                            break;
                                        case "starcraft":
                                            Console.WriteLine("DisplayName: " + subkey.GetValue("DisplayName"));
                                            Console.WriteLine("InstallLocation: " + subkey.GetValue("InstallLocation"));
                                            exeList = GetFiles(subkey.GetValue("InstallLocation").ToString(), ".exe", SearchOption.AllDirectories).Cast<String>().ToList();
                                            exeList.ForEach(i => Console.WriteLine(i));

                                            Console.WriteLine();
                                            break;
                                        case "starcraft ii":
                                            Console.WriteLine("DisplayName: " + subkey.GetValue("DisplayName"));
                                            Console.WriteLine("InstallLocation: " + subkey.GetValue("InstallLocation"));
                                            exeList = GetFiles(subkey.GetValue("InstallLocation").ToString(), ".exe", SearchOption.AllDirectories).Cast<String>().ToList();
                                            exeList.ForEach(i => Console.WriteLine(i));

                                            Console.WriteLine();
                                            break;
                                        case "warcraft iii":
                                            Console.WriteLine("DisplayName: " + subkey.GetValue("DisplayName"));
                                            Console.WriteLine("InstallLocation: " + subkey.GetValue("InstallLocation"));
                                            exeList = GetFiles(subkey.GetValue("InstallLocation").ToString(), ".exe", SearchOption.AllDirectories).Cast<String>().ToList();
                                            exeList.ForEach(i => Console.WriteLine(i));

                                            Console.WriteLine();
                                            break;

                                        case "destiny 2":
                                            Console.WriteLine("DisplayName: " + subkey.GetValue("DisplayName"));
                                            Console.WriteLine("InstallLocation: " + subkey.GetValue("InstallLocation"));
                                            exeList = GetFiles(subkey.GetValue("InstallLocation").ToString(), ".exe", SearchOption.AllDirectories).Cast<String>().ToList();
                                            exeList.ForEach(i => Console.WriteLine(i));

                                            Console.WriteLine();
                                            break;

                                    }

                                }
                            }
                        }
                    }
                }
            }).ContinueWith(tsk =>
            {

            });
        }

        public static IEnumerable<string> GetFiles(string path,
                       string searchPatternExpression = "",
                       SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            Regex reSearchPattern = new Regex(searchPatternExpression, RegexOptions.IgnoreCase);
            return Directory.EnumerateFiles(path, "*", searchOption)
                            .Where(file =>
                                     reSearchPattern.IsMatch(Path.GetExtension(file)));
        }

        private void SearchDirectories(DirectoryInfo dir_info, List<string> dir_list)
        {
            try
            {
                foreach (DirectoryInfo subdir_info in dir_info.GetDirectories())
                {

                    SearchDirectories(subdir_info, dir_list);
                }
            }
            catch
            { }
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
