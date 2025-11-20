# WorkBalance 2030 – Simulador de Bem-Estar no Trabalho (WPF)

Aplicativo **WPF (.NET)** que usa **POO (herança + polimorfismo)** e armazenamento em **arrays** (ordem de inserção) para simular um **Índice de Bem-Estar (0–100)** e um **Nível de Risco de Burnout** (Baixo/Moderado/Alto) para diferentes perfis de trabalho (Remoto, Híbrido e Presencial).

O objetivo é discutir o **futuro do trabalho** a partir da **saúde mental e do equilíbrio entre vida pessoal e profissional**, especialmente em cenários de trabalho remoto e híbrido.

---

## Sumário

- [Arquitetura geral](#arquitetura-geral)
- [Requisitos](#requisitos)
- [Como executar (rápido)](#como-executar-rápido)
- [Estrutura da solução](#estrutura-da-solução)
- [Modelo de domínio (POO)](#modelo-de-domínio-poo)
- [Lógica de simulação](#lógica-de-simulação)
- [Uso do app](#uso-do-app)
- [Decisões técnicas (estilo da aula)](#decisões-técnicas-estilo-da-aula)
- [Rúbrica — como atendemos](#rúbrica--como-atendemos)
- [ODS e Futuro do Trabalho](#ods-e-futuro-do-trabalho)

---

## Arquitetura geral

- **WorkBalance2030** (WPF App)
  - **Models**: classes de domínio (`PerfilTrabalho`, derivados, `ContextoSaude`, `ResultadoBemEstar`, `ModoTrabalho`).
  - **Core**: `SimuladorBemEstar` (lógica de cálculo).
  - **Views (Windows)**:
    - `MainWindow` – cadastro de perfis e controle do buffer.
    - `ResultsWindow` – processamento dos perfis e exibição dos resultados.

Principais conceitos implementados:

- **POO com herança e polimorfismo**:
  - `PerfilTrabalho` (abstrata) + `PerfilRemoto`, `PerfilHibrido`, `PerfilPresencial` (override de `CalcularIndiceBase`).
- **Arrays fixos**:
  - Buffer de `PerfilTrabalho[]` com `_qtd` controlando a quantidade válida.
  - Remoção por posição com **deslocamento à esquerda**, mantendo a ordem de inserção.
- **Code-behind em WPF**:
  - Lógica da interface concentrada em `MainWindow.xaml.cs` e `ResultsWindow.xaml.cs`, no mesmo estilo das aulas.

---

## Requisitos

- **Microsoft Visual Studio 2022 Community**
  - Versão utilizada: `17.14.20`.
- **.NET 8 SDK** (ou versão compatível configurada no projeto).
- Sistema operacional: Windows.

Não há dependências externas (sem banco de dados, sem Docker, sem MVVM).

---

## Como executar (rápido)

1. **Abrir a solução**
   - Abrir o arquivo `WorkBalance2030.sln` no Visual Studio 2022.

2. **Selecionar o projeto de inicialização**
   - Na Solution Explorer, clicar com o botão direito no projeto `WorkBalance2030` → **Definir como projeto de inicialização** (se ainda não estiver).

3. **Build**
   - Menu **Build** → **Build Solution** (ou `Ctrl + Shift + B`).

4. **Executar**
   - Pressionar **F5** para rodar o app.
   - A janela principal (`MainWindow`) será aberta.

---

## Estrutura da solução

```txt
WorkBalance2030/
├─ App.xaml
├─ MainWindow.xaml
├─ MainWindow.xaml.cs
├─ ResultsWindow.xaml
├─ ResultsWindow.xaml.cs
├─ Models/
│  ├─ ModoTrabalho.cs
│  ├─ ContextoSaude.cs
│  ├─ PerfilTrabalho.cs
│  ├─ PerfilRemoto.cs
│  ├─ PerfilHibrido.cs
│  ├─ PerfilPresencial.cs
│  └─ ResultadoBemEstar.cs
└─ Core/
   └─ SimuladorBemEstar.cs
````

---

## Modelo de domínio (POO)

### `ModoTrabalho` (enum)

Enumeração usada para representar o tipo de trabalho:

* `Remoto`
* `Hibrido`
* `Presencial`

### `ContextoSaude`

Representa fatores que impactam o bem-estar do colaborador:

* `TemFilhosPequenos : bool`
* `TrabalhaEmTurnosNoturnos : bool`
* `NivelPressaoPorMetas : int` (0–10)
* `TemApoioPsicologico : bool`

### `PerfilTrabalho` (classe abstrata)

Classe base que representa um perfil genérico de trabalho:

* Propriedades comuns:

  * `Nome : string`
  * `Idade : int`
  * `ModoTrabalho : ModoTrabalho`
  * `CargaHorariaSemanal : int`
  * `HorasSonoNoite : int`
  * `PausasPorDia : int`
  * `IntensidadeUsoTecnologia : int` (0–10)
  * `Contexto : ContextoSaude`
* Método abstrato:

  * `CalcularIndiceBase() : int`
    Cada perfil concreto implementa sua própria lógica.

Inclui também um helper `Clamp(int valor, int minimo, int maximo)` para limitar os valores entre 0 e 100.

### Perfis concretos

* `PerfilRemoto`

  * Parte de um índice base (70) e ajusta de acordo com:

    * Carga horária semanal (acima de 40 horas diminui o índice).
    * Poucas horas de sono (menos de 7 horas reduzem o índice).
    * Quantidade de pausas (aumentam o índice).
    * Uso intenso de tecnologia (penaliza mais no remoto).

* `PerfilHibrido`

  * Índice base (75), combina fatores de remoto e presencial.
  * Penaliza excesso de carga horária e sono ruim.
  * Considera pausas e intensidade de tecnologia com peso intermediário.

* `PerfilPresencial`

  * Índice base (80) e foco maior em:

    * Carga horária acima de 44 horas.
    * Sono abaixo de 7 horas.
    * Pausas por dia.
    * Uso de tecnologia com peso menor.

### `ResultadoBemEstar`

Objeto de saída da simulação, com:

* `NomeColaborador : string`
* `ModoTrabalho : ModoTrabalho`
* `IndiceBemEstar : int` (0–100)
* `NivelRiscoBurnout : string` (`"Baixo"`, `"Moderado"`, `"Alto"`)
* `PlanoSemanal : string` – recomendações textuais.

O método `ToString()` já formata o resultado de forma amigável, pronto para exibição e exportação.

### `SimuladorBemEstar` (Core)

Responsável pela regra de negócio final:

* `Processar(PerfilTrabalho perfil) : ResultadoBemEstar`

  * Chama `perfil.CalcularIndiceBase()`.
  * Ajusta pelo `ContextoSaude`:

    * Turnos noturnos, pressão alta por metas, filhos pequenos com pouco sono.
    * Apoio psicológico soma pontos.
  * Faz **clamp** (0–100).
  * Determina nível de risco:

    * `>= 70` → Baixo
    * `40–69` → Moderado
    * `< 40` → Alto
  * Gera `PlanoSemanal` com recomendações diferentes para cada risco e modo de trabalho.

* `ProcessarLote(PerfilTrabalho[] buffer, int quantidade) : ResultadoBemEstar[]`

  * Percorre o array e processa todos os perfis válidos (0..quantidade-1).

---

## Lógica de simulação

A lógica foi pensada para ser **simples, mas coerente com o tema**:

1. **Índice base** depende do tipo de trabalho:

   * Remoto: mais sensível a uso intenso de tecnologia e falta de pausas.
   * Híbrido: intermediário entre remoto e presencial.
   * Presencial: mais afetado por carga horária alta e turnos.

2. **Ajustes por contexto**:

   * **Turnos noturnos** e **alta pressão por metas** reduzem o índice.
   * **Filhos pequenos + pouco sono** também geram penalização.
   * **Apoio psicológico** adiciona um pequeno bônus.

3. **Classificação do risco de burnout**:

   * Índices altos → risco baixo.
   * Índices médios → risco moderado.
   * Índices muito baixos → risco alto.

4. **Plano semanal**:

   * Para risco **alto**: foco em pausas, sono, limites de horário e atividades relaxantes.
   * Para risco **moderado**: ajustes menores na rotina.
   * Para risco **baixo**: manutenção da rotina com pequenas recomendações.
   * Recomendações específicas são adicionadas conforme o `ModoTrabalho` (remoto, híbrido ou presencial) e se há ou não apoio psicológico.

---

## Uso do app

### 1. Tela de Cadastro / Buffer (`MainWindow`)

* **Campos principais**:

  * Nome, Idade.
  * Modo de trabalho (ComboBox: Remoto, Híbrido, Presencial).
  * Carga horária semanal.
  * Horas de sono por noite.
  * Pausas por dia.
  * Intensidade de uso de tecnologia (0–10).
  * Contexto de saúde:

    * Tem filhos pequenos?
    * Trabalha em turnos noturnos?
    * Tem apoio psicológico?
    * Nível de pressão por metas (0–10).

* **Botões**:

  * **Adicionar ao buffer**
    Valida os campos, cria o objeto `PerfilTrabalho` adequado e guarda no array `_buffer`.
  * **Remover posição**
    O usuário informa a posição (1..N) e o app remove aquele perfil deslocando os demais para a esquerda.
  * **Limpar buffer**
    Zera a quantidade de perfis (`_qtd`) e limpa o preview.
  * **Ir para resultados**
    Abre a `ResultsWindow`, passando o buffer e a quantidade utilizada.

* **Preview do buffer**:

  * `TextBox` somente leitura listando:

    * posição
    * nome
    * modo de trabalho
    * carga horária
    * sono e pausas

### 2. Tela de Resultados (`ResultsWindow`)

* **Botão Processar**

  * Usa o `SimuladorBemEstar` para calcular os resultados de todos os perfis do buffer.
  * Mostra os resultados formatados em um `TextBox` (incluindo índice, risco e plano semanal).

* **Botão Exportar .txt**

  * Abre um `SaveFileDialog`.
  * Salva os resultados em um arquivo `.txt` usando codificação UTF-8.

* **Botão Copiar**

  * Copia o texto de resultados para a área de transferência, facilitando colar em relatórios ou e-mails.

* **Botão Fechar**

  * Fecha a tela de resultados e retorna para a tela principal.

---

## Decisões técnicas (estilo da aula)

* **Arrays em vez de `List<T>`**

  * Buffer implementado como `PerfilTrabalho[]` com capacidade fixa (`50`).
  * Controle manual de índice `_qtd` e remoção com deslocamento à esquerda, reforçando o conteúdo de estruturas básicas.

* **OO explícita**

  * Classe abstrata `PerfilTrabalho` com método `CalcularIndiceBase()` sobrescrito em três subclasses.
  * Uso de **polimorfismo** no `SimuladorBemEstar`, que trabalha com `PerfilTrabalho` sem conhecer o tipo concreto.

* **Code-behind no WPF**

  * Lógica de interface diretamente em `MainWindow.xaml.cs` e `ResultsWindow.xaml.cs`, assim como na calculadora e outros exemplos vistos em aula.

* **Validações simples**

  * `int.TryParse` e verificações de faixa (0–10, valores positivos) para evitar entradas inválidas.

---

## Rúbrica — como atendemos

* **Cobertura do enunciado**

  * App WPF com **2 janelas**, **POO** com herança e polimorfismo, **arrays** com remoção e ordem de inserção, e uma lógica de simulação coerente.

* **Aplicação prática**

  * Foca na **saúde mental e bem-estar** de trabalhadores em cenários remotos/híbridos/presenciais, refletindo problemas reais do futuro do trabalho.

* **Profundidade da lógica**

  * Índice de bem-estar com vários fatores (sono, pausas, carga horária, pressão por metas, filhos pequenos, apoio psicológico).
  * Classificação de risco em 3 níveis e plano semanal de recomendações.

* **Clareza conceitual**

  * Nomes de classes e métodos autoexplicativos em português.
  * Estrutura simples para ser facilmente explicada em apresentação.

* **Documentação**

  * README detalhando arquitetura, modelo de domínio, lógica de simulação, execução e ligação com ODS.

---

## ODS e Futuro do Trabalho

O projeto se conecta principalmente com:

* **ODS 3 – Saúde e Bem-Estar**

  * Aborda diretamente o risco de burnout, a importância do sono, das pausas e do apoio psicológico.

* **ODS 8 – Trabalho Decente e Crescimento Econômico**

  * Discute o equilíbrio entre produtividade e bem-estar em contextos de trabalho remoto, híbrido e presencial.

Além disso, o tema conversa com a discussão de **futuro do trabalho** levantada na disciplina: aumento do trabalho remoto, intensificação do uso de tecnologia, necessidade de cuidar da saúde mental e de repensar a organização do trabalho para evitar sobrecarga e burnout.


- Escrever um **texto curtinho** pra colocar no PDF/relatório explicando o projeto de forma mais “acadêmica” (introdução + conclusão).
