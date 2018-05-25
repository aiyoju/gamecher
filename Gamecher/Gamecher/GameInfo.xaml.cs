using Gamecher.Objects;
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
        public GameInfo(Configuracion game)
        {
            InitializeComponent();
            if (game.juego.descripcion != null) GameDescInfoView.Text = game.juego.descripcion;
            if (game.juego.genero != null) genresOfGameInfo.Text = game.juego.genero;
            if (game.juego.companyia != null) companyOfGameInfo.Text = game.juego.companyia;
            if (game.juego.puntuacion != null) metascoreOfGameInfo.Text = game.juego.puntuacion.ToString();
            if (game.juego.imageUrl != null) ImageOfGameInfo.ImageSource = new BitmapImage(new Uri(game.juego.imageUrl));
        }

        //Close the window when pressing the button
        private void AcceptPressed(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Effect = null;
            Application.Current.MainWindow.Opacity = 1;
            Close();
        }
    }
}
