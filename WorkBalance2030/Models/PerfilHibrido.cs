using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBalance2030.Models
{
    internal class PerfilHibrido : PerfilTrabalho
    {
        public PerfilHibrido() : base(ModoTrabalho.Hibrido)
        {
        }

        public override int CalcularIndiceBase()
        {
            int indice = 75;

            if (CargaHorariaSemanal > 40)
            {
                indice -= (CargaHorariaSemanal - 40);
            }

            if (HorasSonoNoite < 7)
            {
                indice -= (7 - HorasSonoNoite) * 2;
            }

            indice += PausasPorDia * 2;

            // Uso de tecnologia pesa, mas um pouco menos que remoto
            indice -= (int)(IntensidadeUsoTecnologia * 0.5);

            return Clamp(indice, 0, 100);
        }
    }
}

