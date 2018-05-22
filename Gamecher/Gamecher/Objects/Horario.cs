using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gamecher.Objects
{
    public class Horario
    {
        public int idRegistoJuego { get; set; }
        public RegistoJuego registoJuego { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }

        public Horario()
        {
        }

        public Horario(int idRegistoJuego, RegistoJuego registoJuego, DateTime fechaInicio)
        {
            this.idRegistoJuego = idRegistoJuego;
            this.registoJuego = registoJuego;
            this.fechaInicio = fechaInicio;
        }

        public Horario(int idRegistoJuego, RegistoJuego registoJuego, DateTime fechaInicio, DateTime fechaFin)
        {
            this.idRegistoJuego = idRegistoJuego;
            this.registoJuego = registoJuego;
            this.fechaInicio = fechaInicio;
            this.fechaFin = fechaFin;
        }
    }
}
