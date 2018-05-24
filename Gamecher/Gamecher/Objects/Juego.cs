using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gamecher.Objects
{
    public class Juego
    {
        public int? idJuego { get; set; }
        public Plataforma plataforma { get; set; }
        public string nombre { get; set; }
        public string appid { get; set; }
        public string imageUrl { get; set; }
        public string descripcion { get; set; }
        public string genero { get; set; }
        public string companyia { get; set; }
        public string pathConfiguracion { get; set; }
        public sbyte? trofeos { get; set; }
        public DateTimeOffset fechaLanzamiento { get; set; }
        public double? puntuacion { get; set; }
        public DateTimeOffset fechaModificado { get; set; }


        public Juego()
        {
        }

        public Juego(Plataforma plataforma, DateTimeOffset fechaModificado)
        {
            this.plataforma = plataforma;
            this.fechaModificado = fechaModificado;
        }

        public Juego(Plataforma plataforma, string nombre, string appid, string imageUrl, string descripcion, string genero,
                string companyia, string pathConfiguracion, sbyte? trofeos, DateTimeOffset fechaLanzamiento, double? puntuacion,
                DateTimeOffset fechaModificado)
        {
            this.plataforma = plataforma;
            this.nombre = nombre;
            this.appid = appid;
            this.imageUrl = imageUrl;
            this.descripcion = descripcion;
            this.genero = genero;
            this.companyia = companyia;
            this.pathConfiguracion = pathConfiguracion;
            this.trofeos = trofeos;
            this.fechaLanzamiento = fechaLanzamiento;
            this.puntuacion = puntuacion;
            this.fechaModificado = fechaModificado;
        }


    }
}
