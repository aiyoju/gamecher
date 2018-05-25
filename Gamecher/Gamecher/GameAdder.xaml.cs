using Gamecher.Objects;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
                                        favorito = 0,
                                        cuenta = new Cuenta()
                                    };
                                    //TODO Por modificar
                                    config.cuenta.idCuenta = 1;
                                    config.id.idCuenta = 1;

                                    StringContent jsonConfig = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");

                                    string response = HTTPUtils.HTTPPost("http://"+HTTPUtils.IP+":8080/gamecher/configuraciones", jsonConfig);

                                    config = JsonConvert.DeserializeObject<Configuracion>(response);

                                    RegistoJuego registroJuego = new RegistoJuego()
                                    {

                                        juego = config.juego,
                                        cuenta = new Cuenta() { idCuenta = config.cuenta.idCuenta },
                                        horasJugadas = 0
                                    };

                                    StringContent jsonRegistro = new StringContent(JsonConvert.SerializeObject(registroJuego), Encoding.UTF8, "application/json");

                                    string registro = HTTPUtils.HTTPPost("http://" + HTTPUtils.IP + ":8080/gamecher/registro_juegos", jsonRegistro);


                                    string nombre = config.juego.nombre.Replace('\\', '_').Replace('/', '_').Replace(':', '_').Replace('*', '_').Replace('?', '_').Replace('\"', '_').Replace('<', '_').Replace('>', '_').Replace('|', '_').Replace(' ', '_');
                                    File.WriteAllText(@"Data\GamesRegister\" + nombre + ".txt", registro);

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

        public void SaveRegister()
        {
            //TODO Method SaveRegister is empty
        }

        public void SetGamesUI(List<Configuracion> games)
        {
            var listOfCategories = new List<CategoryHelper>();
            var listOfGames = new List<string>();

            int quantityOfButtons = 0;
            int quantityOfFavGames = 0;
            foreach (ButtonFilter buttonFilter in (Application.Current.MainWindow as MainWindow).StackPanel.Children)
            {
                listOfCategories.Add(new CategoryHelper()
                {
                    category = buttonFilter.GenreGames.Text,
                    quantity = Int32.Parse(buttonFilter.numberGames.Text)
                });
                quantityOfButtons++;

            }
            foreach (GameCard gameCard in (Application.Current.MainWindow as MainWindow).WrapPanel.Children)
            {
                listOfGames.Add(gameCard.GameName.Text);
            }

            if (quantityOfButtons > 0) (Application.Current.MainWindow as MainWindow).StackPanel.Children.Clear();

            var gameList = new List<Configuracion>();

            foreach (Configuracion game in games)
            {
                int? posOfGame = null;
                bool hasGame = false;

                for (int i = 0; i < listOfGames.Count; i++)
                {
                    if (listOfGames[i].Equals(game.juego.nombre))
                    {
                        posOfGame = i;
                        hasGame = true;
                        break;
                    }
                }
                if (!hasGame)
                {
                    gameList.Add(game);
                }

            }

            foreach (Configuracion game in gameList)
            {
                Console.WriteLine(JsonConvert.SerializeObject(game));
                string nombre = game.juego.nombre.Replace('\\', '_').Replace('/', '_').Replace(':', '_').Replace('*', '_').Replace('?', '_').Replace('\"', '_').Replace('<', '_').Replace('>', '_').Replace('|', '_').Replace(' ', '_');

                Console.WriteLine(nombre);
                File.WriteAllText(@"Data\SavedGames\" + nombre + ".txt", JsonConvert.SerializeObject(game));

                string[] linesRegistro = File.ReadAllLines(@"Data\GamesRegister\" + nombre + ".txt");
                string jsonCache = "";
                foreach (string line in linesRegistro)
                {
                    jsonCache += line;
                }

                RegistoJuego registro = JsonConvert.DeserializeObject<RegistoJuego>(jsonCache);

                GameCard gC = new GameCard
                {
                    Tag = nombre
                };
                if (game.favorito == 1)
                {
                    gC.FavButton.Style = null;
                }
                gC.hoursPlayed.Text = registro.horasJugadas.ToString();
                gC.FavButton.Tag = nombre;
                gC.PlayButton.MouseUp += PlayButtonPressed;
                gC.FavButton.MouseUp += FavButtonPressed;
                gC.SettingsButton.MouseUp += SettingsButtonPressed;
                gC.ImageGame.ImageSource = new BitmapImage(new Uri(game.juego.imageUrl));
                gC.GameName.Text = game.juego.nombre;
                gC.PlayButton.Tag = game.pathExe;
                gC.SettingsButton.Tag = game.juego.pathConfiguracion;
                (Application.Current.MainWindow as MainWindow).WrapPanel.Children.Add(gC);

                if (game.favorito == 1)
                {
                    quantityOfFavGames++;
                }
                List<string> categories = new List<string>
                {
                    "Favorites",
                    "All Games"
                };

                if (game.juego.genero != null)
                {
                    var categoriesCache = Regex.Split(game.juego.genero, @",").ToList<string>();
                    categoriesCache.RemoveAt(categoriesCache.Count - 1);

                    foreach (var cache in categoriesCache)
                    {
                        categories.Add(cache);
                    }

                    foreach (CategoryHelper category in listOfCategories)
                    {
                        Console.WriteLine(category.category + "   " + category.quantity);
                    }
                    Console.WriteLine();
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
                        }
                        else
                        {
                            listOfCategories.Add(new CategoryHelper()
                            {
                                category = category,
                                quantity = 1
                            });
                        }
                    }

                }
            }

            foreach (CategoryHelper category in listOfCategories)
            {
                if (category.category.Equals("Favorites"))
                {
                    ButtonFilter bFav = new ButtonFilter();
                    bFav.GenreGames.Text = category.category;
                    bFav.numberGames.Text = quantityOfFavGames.ToString();
                    bFav.MouseUp += FilterGame;
                    (Application.Current.MainWindow as MainWindow).StackPanel.Children.Add(bFav);
                }
                else
                {
                    ButtonFilter bF = new ButtonFilter();
                    bF.GenreGames.Text = category.category;
                    bF.numberGames.Text = category.quantity.ToString();
                    bF.MouseUp += FilterGame;
                    (Application.Current.MainWindow as MainWindow).StackPanel.Children.Add(bF);
                }
            }

        }

        public void SettingsButtonPressed(object sender, MouseButtonEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).Opacity = 0.9;
            (Application.Current.MainWindow as MainWindow).Effect = new BlurEffect();
            if ((sender as Image).Tag != null && !(sender as Image).Tag.ToString().Equals(""))
            {

                var configGame = new ConfigGame((sender as Image).Tag.ToString())
                {
                    Owner = (Application.Current.MainWindow as MainWindow),
                    ShowInTaskbar = false
                };

                configGame.ShowDialog();
            }
            else
            {
                MessageBox.Show("Sorry, this game doesn't have a proper configurable file.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                (Application.Current.MainWindow as MainWindow).Opacity = 1;
                (Application.Current.MainWindow as MainWindow).Effect = null;
            }
        }

        public void PlayButtonPressed(object sender, MouseButtonEventArgs e)
        {
            string launchPath = (sender as Polygon).Tag.ToString();
            Task t = Task.Factory.StartNew(() =>
            {
                string folder = System.IO.Path.GetDirectoryName(@"Data\SavedGames\");
                string filter = "*.txt";
                string[] filesCache = Directory.GetFiles(folder, filter);
                foreach (var file in filesCache)
                {
                    Console.WriteLine(file);
                    if (!file.Equals(@"Data\SavedGames\ReadMe.txt"))
                    {
                        string[] lines = File.ReadAllLines(file);
                        string json = "";
                        foreach (string line in lines)
                        {
                            json += line;
                        }
                        Console.WriteLine(json);

                        Configuracion game = JsonConvert.DeserializeObject<Configuracion>(json);

                        if (game.pathExe.Equals(launchPath))
                        {
                            string nombre = game.juego.nombre.Replace('\\', '_').Replace('/', '_').Replace(':', '_').Replace('*', '_').Replace('?', '_').Replace('\"', '_').Replace('<', '_').Replace('>', '_').Replace('|', '_').Replace(' ', '_');
                            string[] linesRegistro = File.ReadAllLines(@"Data\GamesRegister\" + nombre + ".txt");
                            string jsonCache = "";
                            foreach (string line in linesRegistro)
                            {
                                jsonCache += line;
                            }

                            RegistoJuego registro = JsonConvert.DeserializeObject<RegistoJuego>(jsonCache);

                            Horario horario = new Horario()
                            {
                                registoJuego = registro
                            };

                            StringContent jsonRegistro = new StringContent(JsonConvert.SerializeObject(horario), Encoding.UTF8, "application/json");

                            HTTPUtils.HTTPPost("http://" + HTTPUtils.IP + ":8080/gamecher/horarios", jsonRegistro);

                        }
                    }
                }
            }).ContinueWith(tsk =>
                    {
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            Process process = new Process();
                            process.Exited += new EventHandler(UpdateHours);
                            process.StartInfo.FileName = (sender as Polygon).Tag.ToString();
                            //process.StartInfo.FileName = "cmd.exe";
                            process.EnableRaisingEvents = true;
                            process.Start();
                        });
                    });

        }

        public void FilterGame(object sender, MouseButtonEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).FilterGames((sender as ButtonFilter).GenreGames.Text);
        }

        public void FavButtonPressed(object sender, MouseButtonEventArgs e)
        {
            FavGame((sender as Image).Tag.ToString());
        }

        public void UpdateHours(object sender, EventArgs e)
        {
            List<Configuracion> games = new List<Configuracion>();
            Task t = Task.Factory.StartNew(() =>
            {
                string folder = System.IO.Path.GetDirectoryName(@"Data\SavedGames\");
                string filter = "*.txt";
                string[] filesCache = Directory.GetFiles(folder, filter);
                foreach (var file in filesCache)
                {
                    Console.WriteLine(file);
                    if (!file.Equals(@"Data\SavedGames\ReadMe.txt"))
                    {
                        string[] lines = File.ReadAllLines(file);
                        string json = "";
                        foreach (string line in lines)
                        {
                            json += line;
                        }
                        Console.WriteLine(json);

                        Configuracion game = JsonConvert.DeserializeObject<Configuracion>(json);

                        string nombre = game.juego.nombre.Replace('\\', '_').Replace('/', '_').Replace(':', '_').Replace('*', '_').Replace('?', '_').Replace('\"', '_').Replace('<', '_').Replace('>', '_').Replace('|', '_').Replace(' ', '_');
                        string[] linesRegistro = File.ReadAllLines(@"Data\GamesRegister\" + nombre + ".txt");
                        string jsonCache = "";
                        foreach (string line in linesRegistro)
                        {
                            jsonCache += line;
                        }

                        RegistoJuego registro = JsonConvert.DeserializeObject<RegistoJuego>(jsonCache);

                        if (game.pathExe.Equals((sender as Process).StartInfo.FileName))
                        {

                            Horario horario = new Horario();

                            StringContent jsonRegistro = new StringContent(JsonConvert.SerializeObject(horario), Encoding.UTF8, "application/json");

                            string registoHorasJson = HTTPUtils.HTTPPut("http://" + HTTPUtils.IP + ":8080/gamecher/horarios/" + registro.idRegistoJuego, "", jsonRegistro);
                            try
                            {
                                RegistoJuego registoHoras = JsonConvert.DeserializeObject<RegistoJuego>(registoHorasJson);

                                File.WriteAllText(@"Data\GamesRegister\" + nombre + ".txt", JsonConvert.SerializeObject(registoHoras));
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace);
                            }

                        }

                        games.Add(JsonConvert.DeserializeObject<Configuracion>(json));
                    }
                }
            }).ContinueWith(tsk =>
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    (Application.Current.MainWindow as MainWindow).SetHours();
                    SearchForSavedGames();
                });
            });

        }

        public void FavGame(string tag)
        {

            Task t = Task.Factory.StartNew(() =>
            {

                string[] lines = File.ReadAllLines(@"Data\SavedGames\" + tag + ".txt");
                string json = "";
                foreach (string line in lines)
                {
                    json += line;
                }

                Configuracion game = JsonConvert.DeserializeObject<Configuracion>(json);

                if (game.favorito == 0)
                {
                    game.favorito = 1;
                }
                else
                {
                    game.favorito = 0;
                }

                HTTPUtils.HTTPGet("http://" + HTTPUtils.IP + ":8080/gamecher/configuraciones/cuentas/" + game.id.idCuenta + @"/juegos/" + game.id.idJuego + @"/favoritos/" + game.favorito, "");

                File.WriteAllText(@"Data\SavedGames\" + tag + ".txt", JsonConvert.SerializeObject(game));

            }).ContinueWith(tsk =>
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    SearchForSavedGames();
                });
            });
        }

        public void SearchForSavedGames()
        {
            (Application.Current.MainWindow as MainWindow).WrapPanel.Children.Clear();
            (Application.Current.MainWindow as MainWindow).StackPanel.Children.Clear();
            List<Configuracion> games = new List<Configuracion>();
            Task t = Task.Factory.StartNew(() =>
            {
                string folder = System.IO.Path.GetDirectoryName(@"Data\SavedGames\");
                string filter = "*.txt";
                string[] filesCache = Directory.GetFiles(folder, filter);
                foreach (var file in filesCache)
                {
                    Console.WriteLine(file);
                    if (!file.Equals(@"Data\SavedGames\ReadMe.txt"))
                    {
                        string[] lines = File.ReadAllLines(file);
                        string json = "";
                        foreach (string line in lines)
                        {
                            json += line;
                        }
                        Console.WriteLine(json);
                        games.Add(JsonConvert.DeserializeObject<Configuracion>(json));
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

        public void SearchForBattlenetGames()
        {
            List<Configuracion> games = new List<Configuracion>();
            Configuracion config = null;
            StringContent jsonConfig = null;
            RegistoJuego registroJuego = null;
            StringContent jsonRegistro = null;
            string response = null;
            string registro = null;
            string nombre = null;
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
                                                favorito = 0,
                                                cuenta = new Cuenta()
                                            };
                                            //Por modificar
                                            config.cuenta.idCuenta = 1;
                                            config.id.idCuenta = 1;

                                            jsonConfig = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");

                                            response = HTTPUtils.HTTPPost("http://" + HTTPUtils.IP + ":8080/gamecher/configuraciones", jsonConfig);

                                            config = JsonConvert.DeserializeObject<Configuracion>(response);

                                            registroJuego = new RegistoJuego()
                                            {

                                                juego = config.juego,
                                                cuenta = new Cuenta() { idCuenta = config.cuenta.idCuenta },
                                                horasJugadas = 0
                                            };

                                            jsonRegistro = new StringContent(JsonConvert.SerializeObject(registroJuego), Encoding.UTF8, "application/json");

                                            registro = HTTPUtils.HTTPPost("http://" + HTTPUtils.IP + ":8080/gamecher/registro_juegos", jsonRegistro);

                                            nombre = config.juego.nombre.Replace('\\', '_').Replace('/', '_').Replace(':', '_').Replace('*', '_').Replace('?', '_').Replace('\"', '_').Replace('<', '_').Replace('>', '_').Replace('|', '_').Replace(' ', '_');
                                            File.WriteAllText(@"Data\GamesRegister\" + nombre + ".txt", registro);

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
                                                favorito = 0,
                                                cuenta = new Cuenta()
                                            };
                                            //Por modificar
                                            config.cuenta.idCuenta = 1;
                                            config.id.idCuenta = 1;

                                            jsonConfig = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");

                                            response = HTTPUtils.HTTPPost("http://" + HTTPUtils.IP + ":8080/gamecher/configuraciones", jsonConfig);

                                            config = JsonConvert.DeserializeObject<Configuracion>(response);

                                            registroJuego = new RegistoJuego()
                                            {

                                                juego = config.juego,
                                                cuenta = new Cuenta() { idCuenta = config.cuenta.idCuenta },
                                                horasJugadas = 0
                                            };

                                            jsonRegistro = new StringContent(JsonConvert.SerializeObject(registroJuego), Encoding.UTF8, "application/json");

                                            registro = HTTPUtils.HTTPPost("http://" + HTTPUtils.IP + ":8080/gamecher/registro_juegos", jsonRegistro);

                                            nombre = config.juego.nombre.Replace('\\', '_').Replace('/', '_').Replace(':', '_').Replace('*', '_').Replace('?', '_').Replace('\"', '_').Replace('<', '_').Replace('>', '_').Replace('|', '_').Replace(' ', '_');
                                            File.WriteAllText(@"Data\GamesRegister\" + nombre + ".txt", registro);

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
                                                favorito = 0,
                                                cuenta = new Cuenta()
                                            };
                                            //Por modificar
                                            config.cuenta.idCuenta = 1;
                                            config.id.idCuenta = 1;

                                            jsonConfig = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");

                                            response = HTTPUtils.HTTPPost("http://" + HTTPUtils.IP + ":8080/gamecher/configuraciones", jsonConfig);

                                            config = JsonConvert.DeserializeObject<Configuracion>(response);

                                            registroJuego = new RegistoJuego()
                                            {

                                                juego = config.juego,
                                                cuenta = new Cuenta() { idCuenta = config.cuenta.idCuenta },
                                                horasJugadas = 0
                                            };

                                            jsonRegistro = new StringContent(JsonConvert.SerializeObject(registroJuego), Encoding.UTF8, "application/json");

                                            registro = HTTPUtils.HTTPPost("http://" + HTTPUtils.IP + ":8080/gamecher/registro_juegos", jsonRegistro);

                                            nombre = config.juego.nombre.Replace('\\', '_').Replace('/', '_').Replace(':', '_').Replace('*', '_').Replace('?', '_').Replace('\"', '_').Replace('<', '_').Replace('>', '_').Replace('|', '_').Replace(' ', '_');
                                            File.WriteAllText(@"Data\GamesRegister\" + nombre + ".txt", registro);

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
                                                favorito = 0,
                                                cuenta = new Cuenta()
                                            };
                                            //Por modificar
                                            config.cuenta.idCuenta = 1;
                                            config.id.idCuenta = 1;

                                            jsonConfig = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");

                                            response = HTTPUtils.HTTPPost("http://" + HTTPUtils.IP + ":8080/gamecher/configuraciones", jsonConfig);

                                            config = JsonConvert.DeserializeObject<Configuracion>(response);

                                            registroJuego = new RegistoJuego()
                                            {

                                                juego = config.juego,
                                                cuenta = new Cuenta() { idCuenta = config.cuenta.idCuenta },
                                                horasJugadas = 0
                                            };

                                            jsonRegistro = new StringContent(JsonConvert.SerializeObject(registroJuego), Encoding.UTF8, "application/json");

                                            registro = HTTPUtils.HTTPPost("http://" + HTTPUtils.IP + ":8080/gamecher/registro_juegos", jsonRegistro);

                                            Console.WriteLine(registro);
                                            nombre = config.juego.nombre.Replace('\\', '_').Replace('/', '_').Replace(':', '_').Replace('*', '_').Replace('?', '_').Replace('\"', '_').Replace('<', '_').Replace('>', '_').Replace('|', '_').Replace(' ', '_');
                                            File.WriteAllText(@"Data\GamesRegister\" + nombre + ".txt", registro);

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
                                                favorito = 0,
                                                cuenta = new Cuenta()
                                            };
                                            //Por modificar
                                            config.cuenta.idCuenta = 1;
                                            config.id.idCuenta = 1;

                                            jsonConfig = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");

                                            response = HTTPUtils.HTTPPost("http://" + HTTPUtils.IP + ":8080/gamecher/configuraciones", jsonConfig);

                                            config = JsonConvert.DeserializeObject<Configuracion>(response);

                                            registroJuego = new RegistoJuego()
                                            {

                                                juego = config.juego,
                                                cuenta = new Cuenta() { idCuenta = config.cuenta.idCuenta },
                                                horasJugadas = 0
                                            };

                                            jsonRegistro = new StringContent(JsonConvert.SerializeObject(registroJuego), Encoding.UTF8, "application/json");

                                            registro = HTTPUtils.HTTPPost("http://" + HTTPUtils.IP + ":8080/gamecher/registro_juegos", jsonRegistro);

                                            nombre = config.juego.nombre.Replace('\\', '_').Replace('/', '_').Replace(':', '_').Replace('*', '_').Replace('?', '_').Replace('\"', '_').Replace('<', '_').Replace('>', '_').Replace('|', '_').Replace(' ', '_');
                                            File.WriteAllText(@"Data\GamesRegister\" + nombre + ".txt", registro);

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
                                                favorito = 0,
                                                cuenta = new Cuenta()
                                            };
                                            //Por modificar
                                            config.cuenta.idCuenta = 1;
                                            config.id.idCuenta = 1;

                                            jsonConfig = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");

                                            response = HTTPUtils.HTTPPost("http://" + HTTPUtils.IP + ":8080/gamecher/configuraciones", jsonConfig);

                                            config = JsonConvert.DeserializeObject<Configuracion>(response);

                                            registroJuego = new RegistoJuego()
                                            {

                                                juego = config.juego,
                                                cuenta = new Cuenta() { idCuenta = config.cuenta.idCuenta },
                                                horasJugadas = 0
                                            };

                                            jsonRegistro = new StringContent(JsonConvert.SerializeObject(registroJuego), Encoding.UTF8, "application/json");

                                            registro = HTTPUtils.HTTPPost("http://" + HTTPUtils.IP + ":8080/gamecher/registro_juegos", jsonRegistro);

                                            nombre = config.juego.nombre.Replace('\\', '_').Replace('/', '_').Replace(':', '_').Replace('*', '_').Replace('?', '_').Replace('\"', '_').Replace('<', '_').Replace('>', '_').Replace('|', '_').Replace(' ', '_');
                                            File.WriteAllText(@"Data\GamesRegister\" + nombre + ".txt", registro);

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
                                                favorito = 0,
                                                cuenta = new Cuenta()
                                            };
                                            //Por modificar
                                            config.cuenta.idCuenta = 1;
                                            config.id.idCuenta = 1;

                                            jsonConfig = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");

                                            response = HTTPUtils.HTTPPost("http://" + HTTPUtils.IP + ":8080/gamecher/configuraciones", jsonConfig);

                                            config = JsonConvert.DeserializeObject<Configuracion>(response);

                                            registroJuego = new RegistoJuego()
                                            {

                                                juego = config.juego,
                                                cuenta = new Cuenta() { idCuenta = config.cuenta.idCuenta },
                                                horasJugadas = 0
                                            };

                                            jsonRegistro = new StringContent(JsonConvert.SerializeObject(registroJuego), Encoding.UTF8, "application/json");

                                            registro = HTTPUtils.HTTPPost("http://" + HTTPUtils.IP + ":8080/gamecher/registro_juegos", jsonRegistro);

                                            nombre = config.juego.nombre.Replace('\\', '_').Replace('/', '_').Replace(':', '_').Replace('*', '_').Replace('?', '_').Replace('\"', '_').Replace('<', '_').Replace('>', '_').Replace('|', '_').Replace(' ', '_');
                                            File.WriteAllText(@"Data\GamesRegister\" + nombre + ".txt", registro);

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
                                                favorito = 0,
                                                cuenta = new Cuenta()
                                            };
                                            //Por modificar
                                            config.cuenta.idCuenta = 1;
                                            config.id.idCuenta = 1;

                                            jsonConfig = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");

                                            response = HTTPUtils.HTTPPost("http://" + HTTPUtils.IP + ":8080/gamecher/configuraciones", jsonConfig);

                                            config = JsonConvert.DeserializeObject<Configuracion>(response);

                                            registroJuego = new RegistoJuego()
                                            {

                                                juego = config.juego,
                                                cuenta = new Cuenta() { idCuenta = config.cuenta.idCuenta },
                                                horasJugadas = 0
                                            };

                                            jsonRegistro = new StringContent(JsonConvert.SerializeObject(registroJuego), Encoding.UTF8, "application/json");

                                            registro = HTTPUtils.HTTPPost("http://" + HTTPUtils.IP + ":8080/gamecher/registro_juegos", jsonRegistro);

                                            nombre = config.juego.nombre.Replace('\\', '_').Replace('/', '_').Replace(':', '_').Replace('*', '_').Replace('?', '_').Replace('\"', '_').Replace('<', '_').Replace('>', '_').Replace('|', '_').Replace(' ', '_');
                                            File.WriteAllText(@"Data\GamesRegister\" + nombre + ".txt", registro);

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
                                                favorito = 0,
                                                cuenta = new Cuenta()
                                            };
                                            //Por modificar
                                            config.cuenta.idCuenta = 1;
                                            config.id.idCuenta = 1;

                                            jsonConfig = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");

                                            response = HTTPUtils.HTTPPost("http://" + HTTPUtils.IP + ":8080/gamecher/configuraciones", jsonConfig);

                                            config = JsonConvert.DeserializeObject<Configuracion>(response);

                                            registroJuego = new RegistoJuego()
                                            {

                                                juego = config.juego,
                                                cuenta = new Cuenta() { idCuenta = config.cuenta.idCuenta },
                                                horasJugadas = 0
                                            };

                                            jsonRegistro = new StringContent(JsonConvert.SerializeObject(registroJuego), Encoding.UTF8, "application/json");

                                            registro = HTTPUtils.HTTPPost("http://" + HTTPUtils.IP + ":8080/gamecher/registro_juegos", jsonRegistro);

                                            nombre = config.juego.nombre.Replace('\\', '_').Replace('/', '_').Replace(':', '_').Replace('*', '_').Replace('?', '_').Replace('\"', '_').Replace('<', '_').Replace('>', '_').Replace('|', '_').Replace(' ', '_');
                                            File.WriteAllText(@"Data\GamesRegister\" + nombre + ".txt", registro);

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
                                                favorito = 0,
                                                cuenta = new Cuenta()
                                            };
                                            //Por modificar
                                            config.cuenta.idCuenta = 1;
                                            config.id.idCuenta = 1;

                                            jsonConfig = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");

                                            response = HTTPUtils.HTTPPost("http://" + HTTPUtils.IP + ":8080/gamecher/configuraciones", jsonConfig);

                                            config = JsonConvert.DeserializeObject<Configuracion>(response);

                                            registroJuego = new RegistoJuego()
                                            {

                                                juego = config.juego,
                                                cuenta = new Cuenta() { idCuenta = config.cuenta.idCuenta },
                                                horasJugadas = 0
                                            };

                                            jsonRegistro = new StringContent(JsonConvert.SerializeObject(registroJuego), Encoding.UTF8, "application/json");

                                            registro = HTTPUtils.HTTPPost("http://" + HTTPUtils.IP + ":8080/gamecher/registro_juegos", jsonRegistro);

                                            nombre = config.juego.nombre.Replace('\\', '_').Replace('/', '_').Replace(':', '_').Replace('*', '_').Replace('?', '_').Replace('\"', '_').Replace('<', '_').Replace('>', '_').Replace('|', '_').Replace(' ', '_');
                                            File.WriteAllText(@"Data\GamesRegister\" + nombre + ".txt", registro);

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


            // Get the selected file name
            if (result == true)
            {
                new ManualGameConfig(dlg.FileName)
                {
                    Owner = (Application.Current.MainWindow as MainWindow),
                    ShowInTaskbar = false
                }.ShowDialog();
            }
        }

    }

}
