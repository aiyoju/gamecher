﻿using Gamecher.Objects;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;

namespace Gamecher
{
    /// <summary>
    /// Lógica de interacción para UserSettings.xaml
    /// </summary>
    public partial class UserSettings : Window
    {
        public UserSettings()
        {
            InitializeComponent();
            theme.Text = "Dark";

            //Checks for the program saved settings on startup, so everything is the same before closing and opening the program.
            Cuenta preferences = (Application.Current.MainWindow as MainWindow).SetPreferencias();
            if (preferences.preferencia.inicioAutomatico == 1) {
                startWithWindows.IsChecked = true;
            }
            if (preferences.preferencia.actualizacionesAutomaticas == 1)
            {
                autoUpdates.IsChecked = true;
            }
            if (preferences.preferencia.minimizarAlCerrar == 1)
            {
                minimizeOnClose.IsChecked = true;
            }
        }

        //Lets the user drag the window by clicking on the topbar
        private void WindowTopBarClicked(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        //Lets the user close the window by pressig the X.
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Effect = null;
            Application.Current.MainWindow.Opacity = 1;
            Close();
        }

        //TODO Implement the theme changer, so that colors can be either lighter or darker
        private void ThemeChangerClick(object sender, RoutedEventArgs e)
        {
            if ((sender as System.Windows.Controls.Primitives.ToggleButton).IsChecked.Value)
            {
                theme.Text = "Light";
            }
            else
            {
                theme.Text = "Dark";
            }
        }


        //Close the window when cancelling
        private void DeclinePressed(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Effect = null;
            Application.Current.MainWindow.Opacity = 1;
            Close();
        }

        //Executes when the user accepts, saving the variables for the next startup to remain the same.
        private void AcceptPressed(object sender, MouseButtonEventArgs e)
        {
            Cuenta preferences = new Cuenta()
            {
                preferencia = new Preferencia()
            };

            if (startWithWindows.IsChecked == true)
            {
                preferences.preferencia.inicioAutomatico = 1;

                RegistryKey rk = Registry.CurrentUser.OpenSubKey
                       ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                
                rk.SetValue("Gamecher", System.Reflection.Assembly.GetExecutingAssembly().Location);
                
            }
            else
            {
                preferences.preferencia.inicioAutomatico = 0;
                RegistryKey rk = Registry.CurrentUser.OpenSubKey
                     ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                rk.DeleteValue("AppName", false);
            }

            if (autoUpdates.IsChecked == true)
            {
                preferences.preferencia.actualizacionesAutomaticas = 1;
            }
            else { preferences.preferencia.actualizacionesAutomaticas = 0; }

            if (minimizeOnClose.IsChecked == true)
            {
                preferences.preferencia.minimizarAlCerrar = 1;
            }
            else { preferences.preferencia.minimizarAlCerrar = 0; }

            File.WriteAllText(@"Data\userConfig\preferences.txt", JsonConvert.SerializeObject(preferences));
            Application.Current.MainWindow.Effect = null;
            Application.Current.MainWindow.Opacity = 1;
            Close();
        }
    }
}
