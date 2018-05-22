using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gamecher.Objects
{
    public class Configuracion
    {
        public ConfiguracionId id { get; set; }
        public Cuenta cuenta { get; set; }
        public Juego juego { get; set; }
        public string pathExe { get; set; }
        public string pathConfig { get; set; }
        public string resolucion { get; set; }
        public string texturas { get; set; }
        public string filtradoTexturas { get; set; }
        public string antiAliasing { get; set; }
        public string modoVentana { get; set; }
        public string sincronizacionVertical { get; set; }
        public int? fps { get; set; }
        public string sombras { get; set; }
        public string iluminacion { get; set; }
        public string oclusionAmbiental { get; set; }
        public string motionBlur { get; set; }
        public string efectos { get; set; }
        public int? sonidoGeneral { get; set; }
        public int? sonidoFx { get; set; }
        public int? sonidoMusica { get; set; }
        public int? sonidoVoces { get; set; }
        public string idioma { get; set; }
        public sbyte? subtitulos { get; set; }
        public string idiomaSubtitulos { get; set; }
        public double? sensibilidad { get; set; }
        public sbyte? suavizadoRaton { get; set; }
        public sbyte? aceleracionRaton { get; set; }
        public int? fov { get; set; }
        public DateTime fechaModificado { get; set; }

        public Configuracion()
        {
        }

        public Configuracion(ConfiguracionId id, Cuenta cuenta, Juego juego, string pathExe, DateTime fechaModificado)
        {
            this.id = id;
            this.cuenta = cuenta;
            this.juego = juego;
            this.pathExe = pathExe;
            this.fechaModificado = fechaModificado;
        }

        public Configuracion(ConfiguracionId id, Cuenta cuenta, Juego juego, string pathExe, string pathConfig,
                string resolucion, string texturas, string filtradoTexturas, string antiAliasing, string modoVentana,
                string sincronizacionVertical, int? fps, string sombras, string iluminacion, string oclusionAmbiental,
                string motionBlur, string efectos, int? sonidoGeneral, int? sonidoFx, int? sonidoMusica,
                int? sonidoVoces, string idioma, sbyte? subtitulos, string idiomaSubtitulos, double? sensibilidad,
                sbyte? suavizadoRaton, sbyte? aceleracionRaton, int? fov, DateTime fechaModificado)
        {
            this.id = id;
            this.cuenta = cuenta;
            this.juego = juego;
            this.pathExe = pathExe;
            this.pathConfig = pathConfig;
            this.resolucion = resolucion;
            this.texturas = texturas;
            this.filtradoTexturas = filtradoTexturas;
            this.antiAliasing = antiAliasing;
            this.modoVentana = modoVentana;
            this.sincronizacionVertical = sincronizacionVertical;
            this.fps = fps;
            this.sombras = sombras;
            this.iluminacion = iluminacion;
            this.oclusionAmbiental = oclusionAmbiental;
            this.motionBlur = motionBlur;
            this.efectos = efectos;
            this.sonidoGeneral = sonidoGeneral;
            this.sonidoFx = sonidoFx;
            this.sonidoMusica = sonidoMusica;
            this.sonidoVoces = sonidoVoces;
            this.idioma = idioma;
            this.subtitulos = subtitulos;
            this.idiomaSubtitulos = idiomaSubtitulos;
            this.sensibilidad = sensibilidad;
            this.suavizadoRaton = suavizadoRaton;
            this.aceleracionRaton = aceleracionRaton;
            this.fov = fov;
            this.fechaModificado = fechaModificado;
        }
    }
}
