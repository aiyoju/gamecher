using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gamecher.Objects
{
    public class Cuenta
    {
        public int? idCuenta { get; set; }
        public Preferencia preferencia { get; set; }
        public string usuario { get; set; }
        public string contrasenya { get; set; }
        public string correo { get; set; }
        public byte[] avatar { get; set; }
        public DateTime fechaAlta { get; set; }
        public DateTime fechaBaja { get; set; }
        public DateTime fechaModificado { get; set; }

        public Cuenta()
        {
        }

        public Cuenta(Preferencia preferencia, string usuario, string contrasenya, string correo, DateTime fechaAlta,
                DateTime fechaModificado)
        {
            this.preferencia = preferencia;
            this.usuario = usuario;
            this.contrasenya = contrasenya;
            this.correo = correo;
            this.fechaAlta = fechaAlta;
            this.fechaModificado = fechaModificado;
        }

        public Cuenta(Preferencia preferencia, string usuario, string contrasenya, string correo, byte[] avatar,
                DateTime fechaAlta, DateTime fechaBaja, DateTime fechaModificado)
        {
            this.preferencia = preferencia;
            this.usuario = usuario;
            this.contrasenya = contrasenya;
            this.correo = correo;
            this.avatar = avatar;
            this.fechaAlta = fechaAlta;
            this.fechaBaja = fechaBaja;
            this.fechaModificado = fechaModificado;
        }
    }
}
