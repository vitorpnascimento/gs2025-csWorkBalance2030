using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBalance2030.Models
{
    internal class PerfilPresencial : PerfilTrabalho
    {
        public PerfilPresencial() : base(ModoTrabalho.Presencial)
        {
        }

        public override int CalcularIndiceBase()
        {
            int indice = 80;

            if (CargaHorariaSemanal > 44)
            {
                indice -= (CargaHorariaSemanal - 44);
            }

            if (HorasSonoNoite < 7)
            {
                indice -= (7 - HorasSonoNoite) * 2;
            }

            indice += PausasPorDia * 2;

            // No presencial, uso de tecnologia pesa menos
            indice -= (int)(IntensidadeUsoTecnologia * 0.3);

            return Clamp(indice, 0, 100);
        }
    }
}

