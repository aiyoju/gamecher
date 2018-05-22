using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gamecher.Objects
{
    public class Preferencia
    {
        public int? idPreferencia { get; set; }
        public Idioma idioma { get; set; }
        public string tema { get; set; }
        public sbyte? inicioAutomatico { get; set; }
        public sbyte? actualizacionesAutomaticas { get; set; }
        public sbyte? minimizarAlCerrar { get; set; }
        public DateTime fechaModificado { get; set; }

        public Preferencia()
        {
        }

        public Preferencia(Idioma idioma, DateTime fechaModificado)
        {
            this.idioma = idioma;
            this.fechaModificado = fechaModificado;
        }

        public Preferencia(Idioma idioma, string tema, sbyte? inicioAutomatico, sbyte? actualizacionesAutomaticas,
                sbyte? minimizarAlCerrar, DateTime fechaModificado)
        {
            this.idioma = idioma;
            this.tema = tema;
            this.inicioAutomatico = inicioAutomatico;
            this.actualizacionesAutomaticas = actualizacionesAutomaticas;
            this.minimizarAlCerrar = minimizarAlCerrar;
            this.fechaModificado = fechaModificado;
        }
    }
}
