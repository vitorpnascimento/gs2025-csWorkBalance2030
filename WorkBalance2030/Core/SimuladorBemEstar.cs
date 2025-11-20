using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBalance2030.Models;

namespace WorkBalance2030.Core
{
    internal class SimuladorBemEstar
    {
        public ResultadoBemEstar Processar(PerfilTrabalho perfil)
        {
            int indice = perfil.CalcularIndiceBase();
            var contexto = perfil.Contexto;

            // Ajustes por contexto de saúde
            if (contexto.TrabalhaEmTurnosNoturnos)
            {
                indice -= 10;
            }

            if (contexto.NivelPressaoPorMetas >= 7)
            {
                indice -= 10;
            }
            else if (contexto.NivelPressaoPorMetas >= 4)
            {
                indice -= 5;
            }

            if (contexto.TemFilhosPequenos && perfil.HorasSonoNoite < 7)
            {
                indice -= 5;
            }

            if (contexto.TemApoioPsicologico)
            {
                indice += 5;
            }

            indice = Clamp(indice, 0, 100);

            string risco;
            if (indice >= 70)
                risco = "Baixo";
            else if (indice >= 40)
                risco = "Moderado";
            else
                risco = "Alto";

            string plano = GerarPlanoSemanal(perfil, indice, risco);

            return new ResultadoBemEstar
            {
                NomeColaborador = perfil.Nome,
                ModoTrabalho = perfil.ModoTrabalho,
                IndiceBemEstar = indice,
                NivelRiscoBurnout = risco,
                PlanoSemanal = plano
            };
        }

        public ResultadoBemEstar[] ProcessarLote(PerfilTrabalho[] buffer, int quantidade)
        {
            var resultados = new ResultadoBemEstar[quantidade];

            for (int i = 0; i < quantidade; i++)
            {
                resultados[i] = Processar(buffer[i]);
            }

            return resultados;
        }

        private int Clamp(int valor, int minimo, int maximo)
        {
            if (valor < minimo) return minimo;
            if (valor > maximo) return maximo;
            return valor;
        }

        private string GerarPlanoSemanal(PerfilTrabalho perfil, int indice, string risco)
        {
            // Plano simples baseado no modo de trabalho e no risco
            var linhas = new System.Collections.Generic.List<string>();

            if (risco == "Alto")
            {
                linhas.Add("• Estabelecer horário fixo de início e término do trabalho.");
                linhas.Add("• Fazer pelo menos 3 pausas de 10 minutos ao longo do dia.");
                linhas.Add("• Evitar horas extras nesta semana, se possível.");
                linhas.Add("• Separar um tempo diário para uma atividade relaxante (leitura, caminhada, hobby).");
            }
            else if (risco == "Moderado")
            {
                linhas.Add("• Manter 2 pausas de 10 minutos ao longo do dia.");
                linhas.Add("• Tentar garantir pelo menos 7 horas de sono por noite.");
                linhas.Add("• Reservar 1 ou 2 dias na semana para atividades sem tela após o expediente.");
            }
            else // Baixo
            {
                linhas.Add("• Manter a rotina atual de pausas e sono.");
                linhas.Add("• Introduzir pequenas pausas ativas (alongamento, água, respiração).");
            }

            switch (perfil.ModoTrabalho)
            {
                case ModoTrabalho.Remoto:
                    linhas.Add("• Definir um espaço fixo de trabalho em casa, separado da área de descanso.");
                    linhas.Add("• Evitar reuniões consecutivas sem intervalos.");
                    break;
                case ModoTrabalho.Hibrido:
                    linhas.Add("• Organizar dias de trabalho presencial para tarefas que exigem mais interação.");
                    linhas.Add("• Aproveitar dias remotos para foco e concentração, com menos reuniões.");
                    break;
                case ModoTrabalho.Presencial:
                    linhas.Add("• Aproveitar deslocamento para ouvir algo leve ou educativo, sem sobrecarregar.");
                    linhas.Add("• Negociar pausas conforme possível com a liderança.");
                    break;
            }

            if (perfil.Contexto.TemApoioPsicologico)
            {
                linhas.Add("• Manter acompanhamento psicológico como parte da rotina.");
            }
            else
            {
                linhas.Add("• Caso seja possível, buscar canais de apoio (RH, psicólogo, programas de bem-estar).");
            }

            return string.Join(Environment.NewLine, linhas);
        }
    }
}
