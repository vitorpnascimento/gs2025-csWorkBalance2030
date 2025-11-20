using System;
using System.IO;
using System.Text;
using System.Windows;
using WorkBalance2030.Core;
using WorkBalance2030.Models;

namespace WorkBalance2030
{
    public partial class ResultsWindow : Window
    {
        private readonly PerfilTrabalho[] _buffer;
        private readonly int _qtd;
        private readonly SimuladorBemEstar _simulador = new SimuladorBemEstar();
        private ResultadoBemEstar[] _resultados = Array.Empty<ResultadoBemEstar>();

        public ResultsWindow(PerfilTrabalho[] buffer, int qtd)
        {
            InitializeComponent();
            _buffer = buffer;
            _qtd = qtd;
        }

        private void btnProcessar_Click(object sender, RoutedEventArgs e)
        {
            _resultados = _simulador.ProcessarLote(_buffer, _qtd);

            var sb = new StringBuilder();
            foreach (var r in _resultados)
            {
                sb.Append(r.ToString());
            }

            txtResultados.Text = sb.ToString();
        }

        private void btnExportar_Click(object sender, RoutedEventArgs e)
        {
            if (_resultados == null || _resultados.Length == 0)
            {
                MessageBox.Show("Nenhum resultado para exportar. Clique em Processar primeiro.",
                    "Exportar", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Arquivo de texto (*.txt)|*.txt",
                FileName = "WorkBalance_Resultados.txt"
            };

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, txtResultados.Text, Encoding.UTF8);
                MessageBox.Show("Arquivo exportado com sucesso.",
                    "Exportar", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnCopiar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtResultados.Text))
            {
                MessageBox.Show("Nenhum texto para copiar. Clique em Processar primeiro.",
                    "Copiar", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Clipboard.SetText(txtResultados.Text);
            MessageBox.Show("Resultados copiados para a área de transferência.",
                "Copiar", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
