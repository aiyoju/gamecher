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
using System.Windows.Media;
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
        //public static readonly string STEAM_API_KEY = "D297C7CEC2B377B7D4ED0FE086825E28";
        public WrapPanel WrapPanel { get; set; }
        public StackPanel StackPanel { get; set; }

        //Creates the icon for the program to stay on the notification tray of Windows.
        public System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();


        public MainWindow()
        {
            InitializeComponent();

            // Initialize the program on startup, setting all the info to its values.
            WrapPanel = wrapMahepanel;
            StackPanel = StackMahePanel;

            // Loads the game stored on disk
            GameAdder gA = new GameAdder();
            gA.SearchForSavedGames();

            // Loads the hours into its positions
            SetHours();

            // Loads the user preferences
            List<Cuenta> defaultUser = new List<Cuenta>();
            string folder = System.IO.Path.GetDirectoryName(@"Data\userConfig\");
            string extension = "*.txt";
            string[] filesCache = Directory.GetFiles(folder, extension);
            bool hasFile = false;
            foreach (var file in filesCache)
            {
                if (file.Equals(@"Data\userConfig\preferences.txt"))
                {
                    hasFile = true;
                    break;
                }
            }
            // If user doesn't have preferences, creates ones.
            if (!hasFile)
            {
                Cuenta preferences = new Cuenta()
                {
                    preferencia = new Preferencia() { minimizarAlCerrar = 0, actualizacionesAutomaticas = 0, inicioAutomatico = 0 }
                };

                File.WriteAllText(@"Data\userConfig\preferences.txt", JsonConvert.SerializeObject(preferences));
            }

            //Draws the icon of the notification Windows menu.
            ni.Icon = new System.Drawing.Icon(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath + @"\Data\", "Gamecher.ico"));
            ni.Visible = true;
            ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = System.Windows.WindowState.Normal;
                };
            ni.ContextMenu = new System.Windows.Forms.ContextMenu();

            ni.ContextMenu.MenuItems.Add("Exit", (s, e) => { ni.Visible = false; Application.Current.Shutdown(); });

        }

        //Lets the user drag the window by clicking on the topbar, and also maximize it on double click
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

        //Lets the user close the window by pressig the X.
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Cuenta preferences = SetPreferencias();
            if (preferences.preferencia.minimizarAlCerrar == 1)
            {
                this.Hide();
            }
            else
            {
                ni.Visible = false;
                Application.Current.Shutdown();
            }

        }

        //Maximizes the windows when the user clicks the maximize square button
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            AdjustWindowSize();
        }

        //Minimizes the window to the taskbar when the user clicks on the _ button
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // Method that allows the user to adjust the size of the window.
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

        //Opens the profile editor (TODO) when the user clicks on the profile image
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

        //Prompts the menu that allows the user to add games, automatically or manually
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

        // Loads the hours into its positions
        // Basically goes through all the games that are 
        // installed and adds the hours, then sets them.
        public void SetHours()
        {

            double? horas = 0;
            Task t = Task.Factory.StartNew(() =>
            {
                string folder = System.IO.Path.GetDirectoryName(@"Data\GamesRegister\");
                string filter = "*.txt";
                string[] filesCache = Directory.GetFiles(folder, filter);
                foreach (var file in filesCache)
                {
                    if (!file.Equals(@"Data\GamesRegister\ReadMe.txt"))
                    {
                        string[] linesRegistro = File.ReadAllLines(file);
                        string jsonCache = "";
                        foreach (string line in linesRegistro)
                        {
                            jsonCache += line;
                        }

                        RegistoJuego registro = JsonConvert.DeserializeObject<RegistoJuego>(jsonCache);

                        horas += registro.horasJugadas;
                    }
                }
            }).ContinueWith(tsk =>
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    totalHours.Text = horas.ToString();
                });
            });
        }

        // Lets the user filter the games by its categories.
        // Same as last method, goes thorugh all the files and stores their categories
        // separating and counting them so it can later update them on the GUI.
        public void FilterGames(string filter)
        {
            List<Configuracion> games = new List<Configuracion>();
            Task t = Task.Factory.StartNew(() =>
            {
                List<Configuracion> gamesCache = new List<Configuracion>();
                string folder = System.IO.Path.GetDirectoryName(@"Data\SavedGames\");
                string extension = "*.txt";
                string[] filesCache = Directory.GetFiles(folder, extension);
                foreach (var file in filesCache)
                {
                    if (!file.Equals(@"Data\SavedGames\ReadMe.txt"))
                    {
                        string[] lines = File.ReadAllLines(file);
                        string json = "";
                        foreach (string line in lines)
                        {
                            json += line;
                        }
                        gamesCache.Add(JsonConvert.DeserializeObject<Configuracion>(json));
                        gamesCache.ForEach(i => Console.WriteLine(i.juego.nombre));
                    }
                }

                foreach (var cache in gamesCache)
                {

                    List<string> categories = new List<string>();

                    if (cache.favorito == 1)
                    {
                        categories.Add("Favorites");
                    }

                    categories.Add("All Games");

                    var categoriesCache = Regex.Split(cache.juego.genero, @",").ToList<string>();
                    categoriesCache.RemoveAt(categoriesCache.Count - 1);

                    foreach (var cacheCategory in categoriesCache)
                    {
                        categories.Add(cacheCategory);
                    }


                    foreach (var category in categories)
                    {
                        if (category.Equals(filter))
                        {
                            games.Add(cache);
                            break;
                        }
                    }

                }

            }).ContinueWith(tsk =>
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    FilterUI(games);
                });
            });
        }

        // Gets the organized categories from the last method and applies those to the UI, updating it.
        // Generates a Card for each different category and then makes +1 to them until the array is done.
        // Then inserts the card into the UI.
        public void FilterUI(List<Configuracion> games)
        {
            WrapPanel.Children.Clear();

            GameAdder gA = new GameAdder();
            foreach (var game in games)
            {
                string nombre = game.juego.nombre.Replace('\\', '_').Replace('/', '_').Replace(':', '_').Replace('*', '_').Replace('?', '_').Replace('\"', '_').Replace('<', '_').Replace('>', '_').Replace('|', '_').Replace(' ', '_');

                GameCard gC = new GameCard
                {
                    Tag = nombre
                };
                if (game.favorito == 1)
                {
                    gC.FavButton.Style = null;
                }
                gC.FavButton.Tag = nombre;
                gC.PlayButton.MouseUp += gA.PlayButtonPressed;
                gC.FavButton.MouseUp += gA.FavButtonPressed;
                gC.SettingsButton.MouseUp += gA.SettingsButtonPressed;
                gC.GameName.MouseUp += gA.GameNamePressed;
                gC.ImageGame.ImageSource = new BitmapImage(new Uri(game.juego.imageUrl));
                gC.GameName.Text = game.juego.nombre;
                gC.GameName.Tag = nombre;
                gC.PlayButton.Tag = game.pathExe;
                gC.SettingsButton.Tag = game.juego.pathConfiguracion;
                (Application.Current.MainWindow as MainWindow).WrapPanel.Children.Add(gC);
            }
        }

        // Reads the account preferences for using them
        public Cuenta SetPreferencias()
        {
            Cuenta preferences = null;
            try
            {
                string[] lines = File.ReadAllLines(@"Data\userConfig\preferences.txt");
                string json = "";
                foreach (string line in lines)
                {
                    json += line;
                }

                preferences = JsonConvert.DeserializeObject<Cuenta>(json);
            }
            catch
            {
            }
            return preferences;
        }

        
    }
}
