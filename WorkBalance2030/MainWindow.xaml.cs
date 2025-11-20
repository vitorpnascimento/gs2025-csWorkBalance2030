using System;
using System.Text;
using System.Windows;
using WorkBalance2030.Core;
using WorkBalance2030.Models;

namespace WorkBalance2030
{
    public partial class MainWindow : Window
    {
        private readonly PerfilTrabalho[] _buffer = new PerfilTrabalho[50];
        private int _qtd = 0;

        public MainWindow()
        {
            InitializeComponent();
            InicializarComboModoTrabalho();
        }

        private void InicializarComboModoTrabalho()
        {
            cmbModoTrabalho.Items.Add("Remoto");
            cmbModoTrabalho.Items.Add("Híbrido");
            cmbModoTrabalho.Items.Add("Presencial");
            cmbModoTrabalho.SelectedIndex = 0;
        }

        private void btnAdicionarBuffer_Click(object sender, RoutedEventArgs e)
        {
            if (_qtd >= _buffer.Length)
            {
                MessageBox.Show("Buffer cheio. Remova algum perfil para adicionar novos.",
                    "Buffer", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Validações simples
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("Informe o nome.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNome.Focus();
                return;
            }

            if (!int.TryParse(txtIdade.Text, out int idade) || idade <= 0)
            {
                MessageBox.Show("Informe uma idade válida.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtIdade.Focus();
                return;
            }

            if (!int.TryParse(txtCargaHoraria.Text, out int cargaHoraria) || cargaHoraria <= 0)
            {
                MessageBox.Show("Informe uma carga horária válida.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCargaHoraria.Focus();
                return;
            }

            if (!int.TryParse(txtHorasSono.Text, out int horasSono) || horasSono <= 0)
            {
                MessageBox.Show("Informe horas de sono válidas.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtHorasSono.Focus();
                return;
            }

            if (!int.TryParse(txtPausasPorDia.Text, out int pausas) || pausas < 0)
            {
                MessageBox.Show("Informe uma quantidade de pausas válida.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPausasPorDia.Focus();
                return;
            }

            if (!int.TryParse(txtIntensidadeTecnologia.Text, out int intensidadeTec) ||
                intensidadeTec < 0 || intensidadeTec > 10)
            {
                MessageBox.Show("Intensidade de tecnologia deve ser entre 0 e 10.",
                    "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtIntensidadeTecnologia.Focus();
                return;
            }

            if (!int.TryParse(txtPressaoMetas.Text, out int pressaoMetas) ||
                pressaoMetas < 0 || pressaoMetas > 10)
            {
                MessageBox.Show("Pressão por metas deve ser entre 0 e 10.",
                    "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPressaoMetas.Focus();
                return;
            }

            // Cria o contexto
            var contexto = new ContextoSaude
            {
                TemFilhosPequenos = chkFilhosPequenos.IsChecked == true,
                TrabalhaEmTurnosNoturnos = chkTurnosNoturnos.IsChecked == true,
                TemApoioPsicologico = chkApoioPsicologico.IsChecked == true,
                NivelPressaoPorMetas = pressaoMetas
            };

            // Cria o perfil específico
            PerfilTrabalho perfil = CriarPerfilPorModoSelecionado();
            perfil.Nome = txtNome.Text.Trim();
            perfil.Idade = idade;
            perfil.CargaHorariaSemanal = cargaHoraria;
            perfil.HorasSonoNoite = horasSono;
            perfil.PausasPorDia = pausas;
            perfil.IntensidadeUsoTecnologia = intensidadeTec;
            perfil.Contexto = contexto;

            // Insere no buffer
            _buffer[_qtd] = perfil;
            _qtd++;

            AtualizarBufferPreview();
            LimparCamposFormulario();
        }

        private PerfilTrabalho CriarPerfilPorModoSelecionado()
        {
            string? modoTexto = cmbModoTrabalho.SelectedItem as string;

            return modoTexto switch
            {
                "Híbrido" => new PerfilHibrido(),
                "Presencial" => new PerfilPresencial(),
                _ => new PerfilRemoto(),
            };
        }

        private void AtualizarBufferPreview()
        {
            var sb = new StringBuilder();

            for (int i = 0; i < _qtd; i++)
            {
                var p = _buffer[i];
                sb.AppendLine($"{i + 1}. {p.Nome} | {p.ModoTrabalho} | {p.CargaHorariaSemanal}h/sem | Sono: {p.HorasSonoNoite}h | Pausas: {p.PausasPorDia}");
            }

            txtBufferPreview.Text = sb.ToString();
        }

        private void LimparCamposFormulario()
        {
            txtNome.Text = string.Empty;
            txtIdade.Text = string.Empty;
            txtCargaHoraria.Text = string.Empty;
            txtHorasSono.Text = string.Empty;
            txtPausasPorDia.Text = string.Empty;
            txtIntensidadeTecnologia.Text = string.Empty;
            txtPressaoMetas.Text = string.Empty;

            chkFilhosPequenos.IsChecked = false;
            chkTurnosNoturnos.IsChecked = false;
            chkApoioPsicologico.IsChecked = false;

            cmbModoTrabalho.SelectedIndex = 0;
        }

        private void btnRemover_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtRemoverPos.Text, out int pos))
            {
                MessageBox.Show("Informe uma posição numérica.", "Remover", MessageBoxButton.OK, MessageBoxImage.Information);
                txtRemoverPos.Focus();
                return;
            }

            if (pos < 1 || pos > _qtd)
            {
                MessageBox.Show($"Posição inválida. Válido: 1..{_qtd}.", "Remover", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int idx = pos - 1;

            for (int i = idx; i < _qtd - 1; i++)
            {
                _buffer[i] = _buffer[i + 1];
            }

            // _buffer[_qtd - 1] = null;
            _qtd--;

            AtualizarBufferPreview();
            txtRemoverPos.Text = string.Empty;
        }

        private void btnLimparBuffer_Click(object sender, RoutedEventArgs e)
        {
            _qtd = 0;
            txtBufferPreview.Text = string.Empty;
            txtRemoverPos.Text = string.Empty;
        }

        private void btnResultados_Click(object sender, RoutedEventArgs e)
        {
            if (_qtd == 0)
            {
                MessageBox.Show("Nenhum perfil no buffer para processar.",
                    "Resultados", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var janelaResultados = new ResultsWindow(_buffer, _qtd);
            janelaResultados.Owner = this;
            janelaResultados.ShowDialog();
        }
    }
}
