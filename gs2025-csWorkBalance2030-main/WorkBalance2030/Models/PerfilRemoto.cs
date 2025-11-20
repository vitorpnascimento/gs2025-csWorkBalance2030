using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBalance2030.Models
{
    internal class PerfilRemoto : PerfilTrabalho
    {
        public PerfilRemoto() : base(ModoTrabalho.Remoto)
        {
        }

        public override int CalcularIndiceBase()
        {
            int indice = 70;

            // Carga horária acima de 40 horas diminui bem-estar
            if (CargaHorariaSemanal > 40)
            {
                indice -= (CargaHorariaSemanal - 40);
            }

            // Pouco sono (menos de 7h) penaliza
            if (HorasSonoNoite < 7)
            {
                indice -= (7 - HorasSonoNoite) * 3;
            }

            // Pausas ajudam
            indice += PausasPorDia * 2;

            // Uso intenso de tecnologia para remoto pode cansar mais
            indice -= IntensidadeUsoTecnologia;

            return Clamp(indice, 0, 100);
        }
    }
}

