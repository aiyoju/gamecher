using Gamecher.Objects;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Gamecher
{

    /// <summary>
    /// Lógica de interacción para ManualGameConfig.xaml
    /// </summary>
    public partial class ManualGameConfig : Window
    {
        string gamePath = "";

        public ManualGameConfig(string pathOfManualGame)
        {
            InitializeComponent();
            gamePath = pathOfManualGame;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void WindowTopBarClicked(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void DeclinePressed(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void AcceptPressed(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void ImageOfManualGamePressed(object sender, MouseButtonEventArgs e)
        {
            // Create OpenFileDialog 
            OpenFileDialog dlg = new OpenFileDialog
            {
                // Set filter for file extension and default file extension 
                DefaultExt = "*.jpg;*.jpeg;*.png",
                Filter = "All Compatible Files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png|JPG/JPEG Files (*.jpg, *.jpeg)|*.jpg;*.jpeg|PNG Files (*.png)|*.png"
            };


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name
            if (result == true)
            {
                string filename = dlg.FileName;
                string newFilename = null;
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Data\GamesImages\" + System.IO.Path.GetFileName(filename))) {
                    newFilename = AppDomain.CurrentDomain.BaseDirectory + @"Data\GamesImages\" + 1 + System.IO.Path.GetFileName(filename);
                }
                else {
                    newFilename = AppDomain.CurrentDomain.BaseDirectory + @"Data\GamesImages\" +System.IO.Path.GetFileName(filename);
                }

                
                
                File.Copy(filename, newFilename);

                SetManuallyAddedGame(newFilename, gamePath);

            }
        }

        public void SetManuallyAddedGame(string image, string gamePath)
        {
            string nombre = TextBoxNameOfManualGame.Text;
            Task t = Task.Factory.StartNew(() =>
            {

                if (nombre != null || !nombre.Equals(""))
                {
                    Configuracion config = new Configuracion()
                    {
                        pathExe = gamePath,
                        juego = new Juego()
                        {
                            imageUrl = image,
                            nombre = nombre
                        }
                    };
                    File.WriteAllText(@"Data\SavedGames\" + nombre + ".txt", JsonConvert.SerializeObject(config));

                    RegistoJuego register = new RegistoJuego()
                    {
                        cuenta = new Cuenta(),
                        juego = new Juego()
                        {
                            imageUrl = image,
                            nombre = nombre
                        }
                    };
                    File.WriteAllText(@"Data\GamesRegister\" + nombre + ".txt", JsonConvert.SerializeObject(register));
                }

            }).ContinueWith(tsk =>
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    GameAdder gA = new GameAdder();
                    gA.SearchForSavedGames();
                });
            });
        }
    }
}
