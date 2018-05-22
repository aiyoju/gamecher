using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gamecher.Objects
{
    public class Error
    {
        public int? idError { get; set; }
        public Cuenta cuenta { get; set; }
        public string descripcion { get; set; }
        public string excepcion { get; set; }
        public DateTime fecha { get; set; }

        public Error()
        {
        }

        public Error(Cuenta cuenta, DateTime fecha)
        {
            this.cuenta = cuenta;
            this.fecha = fecha;
        }

        public Error(Cuenta cuenta, string descripcion, string excepcion, DateTime fecha)
        {
            this.cuenta = cuenta;
            this.descripcion = descripcion;
            this.excepcion = excepcion;
            this.fecha = fecha;
        }
    }
}
