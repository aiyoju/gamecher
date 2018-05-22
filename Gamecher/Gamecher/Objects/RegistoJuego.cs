using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gamecher.Objects
{
    public class RegistoJuego
    {
        public int? idRegistoJuego { get; set; }
        public Cuenta cuenta { get; set; }
        public Juego juego { get; set; }
        public string trofeosCompledatos { get; set; }
        public sbyte? instalado { get; set; }
        public DateTime fechaModificado { get; set; }
        public double? horasJugadas { get; set; }

        public RegistoJuego()
        {
        }

        public RegistoJuego(Cuenta cuenta, Juego juego, DateTime fechaModificado)
        {
            this.cuenta = cuenta;
            this.juego = juego;
            this.fechaModificado = fechaModificado;
        }

        public RegistoJuego(Cuenta cuenta, Juego juego, string trofeosCompledatos, sbyte? instalado, DateTime fechaModificado,
                double? horasJugadas)
        {
            this.cuenta = cuenta;
            this.juego = juego;
            this.trofeosCompledatos = trofeosCompledatos;
            this.instalado = instalado;
            this.fechaModificado = fechaModificado;
            this.horasJugadas = horasJugadas;
        }
    }
}
