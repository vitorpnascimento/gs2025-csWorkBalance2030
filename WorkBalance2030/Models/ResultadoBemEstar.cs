using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBalance2030.Models
{
    internal class ResultadoBemEstar
    {
        public string NomeColaborador { get; set; } = string.Empty;
        public ModoTrabalho ModoTrabalho { get; set; }
        public int IndiceBemEstar { get; set; }
        public string NivelRiscoBurnout { get; set; } = string.Empty;
        public string PlanoSemanal { get; set; } = string.Empty;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Nome: {NomeColaborador}");
            sb.AppendLine($"Modo de trabalho: {ModoTrabalho}");
            sb.AppendLine($"Índice de bem-estar: {IndiceBemEstar}");
            sb.AppendLine($"Risco de burnout: {NivelRiscoBurnout}");
            sb.AppendLine("Plano semanal sugerido:");
            sb.AppendLine(PlanoSemanal);
            sb.AppendLine("----------------------------------------");
            return sb.ToString();
        }
    }
}
