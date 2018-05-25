using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
        public ManualGameConfig(string pathOfManualGame)
        {
            InitializeComponent();
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
            }
        }
    }
}
