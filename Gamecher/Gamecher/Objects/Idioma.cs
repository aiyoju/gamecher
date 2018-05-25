using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gamecher.Objects
{
    public class Idioma
    {
        public int? idIdioma { get; set; }
        public string siglas { get; set; }
        public string nombre { get; set; }
        public DateTimeOffset fechaModificado { get; set; }

        public Idioma()
        {
        }

        public Idioma(string siglas, string nombre, DateTime fechaModificado)
        {
            this.siglas = siglas;
            this.nombre = nombre;
            this.fechaModificado = fechaModificado;
        }
    }
}
