using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gamecher.Objects
{
    public class ConfiguracionId
    {
        public int idCuenta { get; set; }
        public int idJuego { get; set; }

        public ConfiguracionId()
        {
        }

        public ConfiguracionId(int idCuenta, int idJuego)
        {
            this.idCuenta = idCuenta;
            this.idJuego = idJuego;
        }

        public bool equals(object other)
        {
            if ((this == other))
                return true;
            if ((other == null))
                return false;
            if (!(other is ConfiguracionId))
			return false;
            ConfiguracionId castOther = (ConfiguracionId)other;

            return (this.idCuenta == castOther.idCuenta) && (this.idJuego == castOther.idJuego);
        }

        public int hashCode()
        {
            int result = 17;

            result = 37 * result + this.idCuenta;
            result = 37 * result + this.idJuego;
            return result;
        }
    }
}
