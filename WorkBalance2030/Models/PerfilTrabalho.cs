using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBalance2030.Models
{
    public abstract class PerfilTrabalho
    {
        public string Nome { get; set; } = string.Empty;
        public int Idade { get; set; }

        public ModoTrabalho ModoTrabalho { get; protected set; }

        public int CargaHorariaSemanal { get; set; }      // horas/semana
        public int HorasSonoNoite { get; set; }           // horas/noite
        public int PausasPorDia { get; set; }             // número de pausas
        public int IntensidadeUsoTecnologia { get; set; } // 0 a 10

        public ContextoSaude Contexto { get; set; } = new ContextoSaude();

        protected PerfilTrabalho(ModoTrabalho modoTrabalho)
        {
            ModoTrabalho = modoTrabalho;
        }

        // Cada tipo de perfil calcula um índice base diferente
        public abstract int CalcularIndiceBase();

        protected int Clamp(int valor, int minimo, int maximo)
        {
            if (valor < minimo) return minimo;
            if (valor > maximo) return maximo;
            return valor;
        }
    }
}

