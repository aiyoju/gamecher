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
    /// Lógica de interacción para GameInfo.xaml
    /// </summary>
    public partial class GameInfo : Window
    {
        public GameInfo()
        {
            InitializeComponent();
        }

        private void AcceptPressed(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Effect = null;
            Application.Current.MainWindow.Opacity = 1;
            Close();
        }
    }
}
