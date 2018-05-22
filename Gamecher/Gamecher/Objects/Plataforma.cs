using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gamecher.Objects
{
    public class Plataforma
    {
        public int? idPlataforma { get; set; }
        public string nombre { get; set; }
        public string path { get; set; }
        public string api { get; set; }
        public DateTime fechaModificado { get; set; }

        public Plataforma()
        {
        }

        public Plataforma(DateTime fechaModificado)
        {
            this.fechaModificado = fechaModificado;
        }

        public Plataforma(string nombre, string path, string api, DateTime fechaModificado)
        {
            this.nombre = nombre;
            this.path = path;
            this.api = api;
            this.fechaModificado = fechaModificado;
        }
    }
}
