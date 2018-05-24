using Gamecher.Objects;
using Microsoft.Win32;
using Newtonsoft.Json;
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
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Gamecher
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class GameAdder : Window
    {
        public GameAdder()
        {
            InitializeComponent();
        }

        public void SearchForSteamGames()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            List<string> pathsFound = new List<string>();
            List<string> allAppidFiles = new List<string>();
            List<string> allAppids = new List<string>();
            List<Configuracion> games = new List<Configuracion>();
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
                                var jsonArray = Regex.Split(HTTPUtils.HTTPGet("https://store.steampowered.com/api/appdetails", "?appids=" + appid + "&l=english"), @"""");
                                for (int j = 2; j < jsonArray.Length; j++)
                                {
                                    json += @"""" + jsonArray[j];
                                }

                                var steamGame = JsonConvert.DeserializeObject<RootObject>(json);

                                if (steamGame.app.success)
                                {
                                    Plataforma steam = new Plataforma
                                    {
                                        idPlataforma = 1,
                                        nombre = "Steam",
                                        path = "steamapps",
                                        api = "http://api.steampowered.com/"
                                    };

                                    string genres = "";
                                    bool hasMultiplayer = false;
                                    bool hasCoop = false;
                                    sbyte? hasAchivements = 0;
                                    double? score = null;

                                    if (steamGame.app.data.categories != null)
                                    {
                                        for (int l = 0; l < steamGame.app.data.categories.Count; l++)
                                        {
                                            //Multi-player = id(1)(27)(36)(37), Co-op = id(9)(38)(39), Single-player = id(2)
                                            var categoryCheker = steamGame.app.data.categories[l].id;
                                            if ((categoryCheker == 1 || categoryCheker == 27 || categoryCheker == 36 || categoryCheker == 37) && !hasMultiplayer)
                                            {
                                                genres += "Multi-player,";
                                                hasMultiplayer = true;
                                            }
                                            if ((categoryCheker == 9 || categoryCheker == 38 || categoryCheker == 39) && !hasCoop)
                                            {
                                                genres += "Co-op,";
                                                hasCoop = true;
                                            }
                                            if (categoryCheker == 2)
                                            {
                                                genres += "Single-player,";
                                            }
                                        }
                                    }
                                    if (steamGame.app.data.achievements != null)
                                    {
                                        if (steamGame.app.data.achievements.total > 0)
                                        {
                                            hasAchivements = 1;
                                        }
                                    }
                                    if (steamGame.app.data.genres != null)
                                    {
                                        for (int l = 0; l < steamGame.app.data.genres.Count; l++)
                                        {
                                            genres += steamGame.app.data.genres[l].description + ",";
                                        }
                                    }
                                    if (steamGame.app.data.metacritic != null)
                                    {
                                        score = steamGame.app.data.metacritic.score;
                                    }
                                    Juego game = new Juego
                                    {

                                        plataforma = steam,
                                        nombre = steamGame.app.data.name,
                                        appid = appid,
                                        imageUrl = "https://steamcdn-a.akamaihd.net/steam/apps/" + appid + "/header.jpg",
                                        descripcion = steamGame.app.data.about_the_game,
                                        genero = genres,
                                        companyia = steamGame.app.data.publishers[0],
                                        trofeos = hasAchivements,
                                        puntuacion = score

                                    };

                                    //TODO Si hay una cuenta hara este meteodo, esta por modificar
                                    if (true) { }
                                    Configuracion config = new Configuracion
                                    {
                                        id = new ConfiguracionId(),
                                        juego = game,
                                        pathExe = "steam://rungameid/" + appid,
                                        cuenta = new Cuenta()
                                    };
                                    //Por modificar
                                    config.cuenta.idCuenta = 1;
                                    config.id.idCuenta = 1;

                                    StringContent jsonConfig = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");

                                    string response = HTTPUtils.HTTPPost("http://83.52.124.186:8080/gamecher/configuraciones", jsonConfig);

                                    config = JsonConvert.DeserializeObject<Configuracion>(response);

                                    games.Add(config);
                                }
                            }

                            allappidfiles.ForEach(i => allAppids.Add(i));
                        }
                    }
                }).ContinueWith(tsk =>
                {
                    numberOfTask++;
                    if (numberOfTask == drives.Length)
                    {
                        //pathsFound.ForEach(i => Console.WriteLine(i));
                        //allAppidFiles.ForEach(i => Console.WriteLine(i));
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            SetGamesUI(games);
                        });
                    }
                });
            }
        }

        public void SetGamesUI(List<Configuracion> games)
        {
            var listOfCategories = new List<CategoryHelper>();
            int quantityOfButtons = 0;
            foreach (ButtonFilter buttonFilter in (Application.Current.MainWindow as MainWindow).stackPanel.Children)
            {
                listOfCategories.Add(new CategoryHelper()
                {
                    category = buttonFilter.GenreGames.Text,
                    quantity = Int32.Parse(buttonFilter.numberGames.Text)
                });
                quantityOfButtons++;
            }
            if (quantityOfButtons > 0) (Application.Current.MainWindow as MainWindow).stackPanel.Children.Clear();

            foreach (Configuracion game in games)
            {
                Console.WriteLine(game.juego.nombre);
                GameCard gC = new GameCard();
                gC.ImageGame.ImageSource = new BitmapImage(new Uri(game.juego.imageUrl));
                gC.GameName.Text = game.juego.nombre;
                gC.PlayButton.Tag = game.pathExe;
                gC.PlayButton.MouseUp += LaunchPath;
                (Application.Current.MainWindow as MainWindow).wrapPanel.Children.Add(gC);

                var categories = Regex.Split(game.juego.genero, @",").ToList<string>();
                categories.RemoveAt(categories.Count-1);
                
                foreach (CategoryHelper category in listOfCategories)
                {
                    Console.WriteLine(category.category + "   " + category.quantity);
                }
                Console.WriteLine("\n\n\n\n\n\n\n\n");
                foreach (string category in categories)
                {
                    int? posOfCategory = null;
                    bool hasCategory = false;

                    for (int i = 0; i < listOfCategories.Count; i++)
                    {
                        if (listOfCategories[i].category.Equals(category))
                        {
                            posOfCategory = i;
                            hasCategory = true;
                            break;
                        }
                    }
                    if (hasCategory)
                    {
                        listOfCategories[posOfCategory.Value].quantity++;
                    } else
                    {
                        listOfCategories.Add(new CategoryHelper()
                        {
                            category = category,
                            quantity = 1
                        });
                    }
                }
                                   
            }
            foreach (CategoryHelper category in listOfCategories)
            {
                ButtonFilter bF = new ButtonFilter();
                bF.GenreGames.Text = category.category;
                bF.numberGames.Text = category.quantity.ToString();
                (Application.Current.MainWindow as MainWindow).stackPanel.Children.Add(bF);
            }

        }

        public void LaunchPath(object sender, MouseButtonEventArgs e)
        {
            Process.Start((sender as Polygon).Tag.ToString());
        }

        public void SearchForBattlenetGames()
        {
            List<Configuracion> games = new List<Configuracion>();
            Configuracion config = null;
            StringContent jsonConfig = null;
            string response = null;
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
                                    switch (subkey.GetValue("DisplayName").ToString().ToLower())
                                    {
                                        case "overwatch":

                                            //TODO Si hay una cuenta hara este meteodo, esta por modificar
                                            if (true) { }

                                            config = new Configuracion
                                            {
                                                id = new ConfiguracionId(),
                                                juego = new Juego { idJuego = 12 },
                                                pathExe = "battlenet://Pro",
                                                cuenta = new Cuenta()
                                            };
                                            //Por modificar
                                            config.cuenta.idCuenta = 1;
                                            config.id.idCuenta = 1;

                                            jsonConfig = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");

                                            response = HTTPUtils.HTTPPost("http://83.52.124.186:8080/gamecher/configuraciones", jsonConfig);

                                            config = JsonConvert.DeserializeObject<Configuracion>(response);
                                            games.Add(config);

                                            break;
                                        case "hearthstone":

                                            //TODO Si hay una cuenta hara este meteodo, esta por modificar
                                            if (true) { }

                                            config = new Configuracion
                                            {
                                                id = new ConfiguracionId(),
                                                juego = new Juego { idJuego = 11 },
                                                pathExe = "battlenet://WTCG",
                                                cuenta = new Cuenta()
                                            };
                                            //Por modificar
                                            config.cuenta.idCuenta = 1;
                                            config.id.idCuenta = 1;

                                            jsonConfig = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");

                                            response = HTTPUtils.HTTPPost("http://83.52.124.186:8080/gamecher/configuraciones", jsonConfig);

                                            config = JsonConvert.DeserializeObject<Configuracion>(response);
                                            games.Add(config);

                                            break;
                                        case "heroes of the storm":

                                            //TODO Si hay una cuenta hara este meteodo, esta por modificar
                                            if (true) { }

                                            config = new Configuracion
                                            {
                                                id = new ConfiguracionId(),
                                                juego = new Juego { idJuego = 13 },
                                                pathExe = "battlenet://Hero",
                                                cuenta = new Cuenta()
                                            };
                                            //Por modificar
                                            config.cuenta.idCuenta = 1;
                                            config.id.idCuenta = 1;

                                            jsonConfig = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");

                                            response = HTTPUtils.HTTPPost("http://83.52.124.186:8080/gamecher/configuraciones", jsonConfig);

                                            config = JsonConvert.DeserializeObject<Configuracion>(response);
                                            games.Add(config);

                                            break;
                                        case "diablo iii":

                                            //TODO Si hay una cuenta hara este meteodo, esta por modificar
                                            if (true) { }

                                            config = new Configuracion
                                            {
                                                id = new ConfiguracionId(),
                                                juego = new Juego { idJuego = 14 },
                                                pathExe = "battlenet://D3",
                                                cuenta = new Cuenta()
                                            };
                                            //Por modificar
                                            config.cuenta.idCuenta = 1;
                                            config.id.idCuenta = 1;

                                            jsonConfig = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");

                                            response = HTTPUtils.HTTPPost("http://83.52.124.186:8080/gamecher/configuraciones", jsonConfig);

                                            config = JsonConvert.DeserializeObject<Configuracion>(response);
                                            games.Add(config);

                                            break;
                                        case "diablo ii":

                                            //TODO Si hay una cuenta hara este meteodo, esta por modificar
                                            if (true) { }

                                            config = new Configuracion
                                            {
                                                id = new ConfiguracionId(),
                                                juego = new Juego { idJuego = 15 },
                                                pathExe = "battlenet://D2",
                                                cuenta = new Cuenta()
                                            };
                                            //Por modificar
                                            config.cuenta.idCuenta = 1;
                                            config.id.idCuenta = 1;

                                            jsonConfig = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");

                                            response = HTTPUtils.HTTPPost("http://83.52.124.186:8080/gamecher/configuraciones", jsonConfig);

                                            config = JsonConvert.DeserializeObject<Configuracion>(response);
                                            games.Add(config);

                                            break;
                                        case "world of warcraft":

                                            //TODO Si hay una cuenta hara este meteodo, esta por modificar
                                            if (true) { }

                                            config = new Configuracion
                                            {
                                                id = new ConfiguracionId(),
                                                juego = new Juego { idJuego = 16 },
                                                pathExe = "battlenet://WoW",
                                                cuenta = new Cuenta()
                                            };
                                            //Por modificar
                                            config.cuenta.idCuenta = 1;
                                            config.id.idCuenta = 1;

                                            jsonConfig = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");

                                            response = HTTPUtils.HTTPPost("http://83.52.124.186:8080/gamecher/configuraciones", jsonConfig);

                                            config = JsonConvert.DeserializeObject<Configuracion>(response);
                                            games.Add(config);

                                            break;
                                        case "starcraft":

                                            //TODO Si hay una cuenta hara este meteodo, esta por modificar
                                            if (true) { }

                                            config = new Configuracion
                                            {
                                                id = new ConfiguracionId(),
                                                juego = new Juego { idJuego = 17 },
                                                pathExe = "battlenet://S1",
                                                cuenta = new Cuenta()
                                            };
                                            //Por modificar
                                            config.cuenta.idCuenta = 1;
                                            config.id.idCuenta = 1;

                                            jsonConfig = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");

                                            response = HTTPUtils.HTTPPost("http://83.52.124.186:8080/gamecher/configuraciones", jsonConfig);

                                            config = JsonConvert.DeserializeObject<Configuracion>(response);
                                            games.Add(config);

                                            break;
                                        case "starcraft ii":

                                            //TODO Si hay una cuenta hara este meteodo, esta por modificar
                                            if (true) { }

                                            config = new Configuracion
                                            {
                                                id = new ConfiguracionId(),
                                                juego = new Juego { idJuego = 18 },
                                                pathExe = "battlenet://S2",
                                                cuenta = new Cuenta()
                                            };
                                            //Por modificar
                                            config.cuenta.idCuenta = 1;
                                            config.id.idCuenta = 1;

                                            jsonConfig = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");

                                            response = HTTPUtils.HTTPPost("http://83.52.124.186:8080/gamecher/configuraciones", jsonConfig);

                                            config = JsonConvert.DeserializeObject<Configuracion>(response);
                                            games.Add(config);

                                            break;
                                        case "warcraft iii":

                                            //TODO Si hay una cuenta hara este meteodo, esta por modificar
                                            if (true) { }

                                            config = new Configuracion
                                            {
                                                id = new ConfiguracionId(),
                                                juego = new Juego { idJuego = 19 },
                                                pathExe = "battlenet://W3",
                                                cuenta = new Cuenta()
                                            };
                                            //Por modificar
                                            config.cuenta.idCuenta = 1;
                                            config.id.idCuenta = 1;

                                            jsonConfig = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");

                                            response = HTTPUtils.HTTPPost("http://83.52.124.186:8080/gamecher/configuraciones", jsonConfig);

                                            config = JsonConvert.DeserializeObject<Configuracion>(response);
                                            games.Add(config);

                                            break;

                                        case "destiny 2":

                                            //TODO Si hay una cuenta hara este meteodo, esta por modificar
                                            if (true) { }

                                            config = new Configuracion
                                            {
                                                id = new ConfiguracionId(),
                                                juego = new Juego { idJuego = 20 },
                                                pathExe = "battlenet://DST2",
                                                cuenta = new Cuenta()
                                            };
                                            //Por modificar
                                            config.cuenta.idCuenta = 1;
                                            config.id.idCuenta = 1;

                                            jsonConfig = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");

                                            response = HTTPUtils.HTTPPost("http://83.52.124.186:8080/gamecher/configuraciones", jsonConfig);

                                            config = JsonConvert.DeserializeObject<Configuracion>(response);
                                            games.Add(config);

                                            break;

                                    }

                                }
                            }
                        }
                    }
                }
            }).ContinueWith(tsk =>
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    SetGamesUI(games);
                });
            });
        }

        public static IEnumerable<string> GetFiles(string path,
                       string searchPatternExpression = "",
                       SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            Regex reSearchPattern = new Regex(searchPatternExpression, RegexOptions.IgnoreCase);
            return Directory.EnumerateFiles(path, "*", searchOption)
                            .Where(file =>
                                     reSearchPattern.IsMatch(System.IO.Path.GetExtension(file)));
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

        private void WindowTopBarClicked(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Effect = null;
            Application.Current.MainWindow.Opacity = 1;
            Close();
        }

        private void SearchForGamesAuto(object sender, RoutedEventArgs e)
        {
            SearchForBattlenetGames();
            SearchForSteamGames();
            Application.Current.MainWindow.Effect = null;
            Application.Current.MainWindow.Opacity = 1;
            Close();
        }

        private void SearchForGamesManual(object sender, RoutedEventArgs e)
        {

            // Create OpenFileDialog 
            OpenFileDialog dlg = new OpenFileDialog
            {
                // Set filter for file extension and default file extension 
                DefaultExt = ".exe",
                Filter = "All Compatible Files (*.exe, *.lnk, *.url)|*.exe;*.lnk;*.url|EXE Files (*.exe)|*.exe|LNK Files (*.lnk)|*.lnk|URL Files (*.url)|*.url"
            };


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                Console.WriteLine(filename);
            }
        }

    }

}
