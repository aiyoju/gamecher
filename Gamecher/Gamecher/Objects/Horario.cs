using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gamecher.Objects
{
    public class Horario
    {
        public int? idHorario { get; set; }
        public RegistoJuego registoJuego { get; set; }
        public DateTimeOffset fechaInicio { get; set; }
        public DateTimeOffset fechaFin { get; set; }

        public Horario()
        {
        }

        public Horario(RegistoJuego registoJuego, DateTimeOffset fechaInicio)
        {
            this.registoJuego = registoJuego;
            this.fechaInicio = fechaInicio;
        }

        public Horario(RegistoJuego registoJuego, DateTimeOffset fechaInicio, DateTimeOffset fechaFin)
        {
            this.registoJuego = registoJuego;
            this.fechaInicio = fechaInicio;
            this.fechaFin = fechaFin;
        }
    }
}
