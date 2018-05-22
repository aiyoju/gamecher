using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gamecher.Objects
{
        public class PathSteam
        {
            private int? idPreferencia { get; set; }
            private Preferencia preferencia { get; set; }
            private string pathSteam { get; set; }

            public PathSteam()
            {
            }

            public PathSteam(Preferencia preferencia)
            {
                this.preferencia = preferencia;
            }

            public PathSteam(Preferencia preferencia, string pathSteam)
            {
                this.preferencia = preferencia;
                this.pathSteam = pathSteam;
            }
        }
}
