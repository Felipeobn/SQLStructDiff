# SQLStructDiff — Documentação do Projeto / Handoff

> Documento para continuar o desenvolvimento em outra máquina (inclusive em outra
> conversa com o Claude Code). É **autocontido**: descreve o que é o sistema, as
> decisões tomadas, a arquitetura, o estado atual e o que falta.

## 1. O que é

Aplicação **desktop C# WinForms** (.NET 9, Windows) que compara a **estrutura de
dois bancos SQL Server** (banco A × banco B) e gera scripts de sincronização.

Compara 5 tipos de objeto: **Tabelas, Views, Procedures, Índices, Triggers**.

Fluxo: conectar nos 2 bancos → extrair schema → comparar → ver diferenças com diff
visual (estilo WinMerge) → gerar/executar script de sincronização A→B ou B→A.

## 2. Decisões já tomadas (não reabrir sem motivo)

- **SGBD:** apenas SQL Server (sem Postgres/MySQL por ora).
- **Sincronização:** gera script DDL; o usuário pode **só gerar** (copiar/salvar .sql)
  ou **executar** no alvo (sob confirmação, em transação com rollback).
- **Sem carimbo de data:** Views/Procedures vêm de `sys.sql_modules.definition`
  (não tem data); Tabelas/Índices são reconstruídos do catálogo `sys.*`. Isso evita
  que data de geração dispare diferença falsa.
- **UI no padrão Designer do Visual Studio:** cada Form/UserControl é `partial`,
  com a lógica em `X.cs` e o layout gerado em `X.Designer.cs` (`InitializeComponent`).
  Assim as telas **abrem no designer visual**. Não usamos `.resx` (nenhuma tela tem
  recursos embutidos ainda). *Histórico: a UI era só código, sem `.Designer.cs`, por
  versionabilidade — foi convertida para o padrão Designer a pedido do usuário.*
- **Construtores para o designer:** telas com dependências (MainForm, ParametersForm,
  ScriptPreviewForm) têm um construtor **sem parâmetros** (usado só pelo designer, que
  chama `InitializeComponent`); o construtor real encadeia nele (`: this()`).
  `ConnectionForm` só carrega settings fora do design-time (guarda
  `LicenseManager.UsageMode`). `ConnectionPanel` (agora `UserControl` com um `GroupBox`
  interno) recebe as dependências por `Initialize(...)`, não pelo construtor.
- **Stack genérica do CLAUDE.md pai (.NET + Angular) NÃO se aplica** — este projeto
  é WinForms desktop, sem Angular/web.

## 3. Estrutura da solução

```
SQLStructDiff.sln
├─ SQLStructDiff.csproj         (raiz) WinForms net9.0-windows — camada UI
│  ├─ Program.cs               -> inicia ConnectionForm
│  └─ UI/                      (telas no padrão Designer: X.cs + X.Designer.cs)
│     ├─ ConnectionForm.cs     tela inicial: 2 painéis + Parâmetros + Comparar
│     ├─ ConnectionPanel.cs    UserControl: perfil de servidor + combo de databases
│     ├─ MainForm.cs           árvore de objetos + diff visual + scripts
│     ├─ ScriptPreviewForm.cs  preview do script: Copiar/Salvar/Executar
│     ├─ ParametersForm.cs     parâmetros de comparação
│     └─ Prompt.cs             diálogo simples de input de texto
└─ SQLStructDiff.Core/          biblioteca net9.0-windows — lógica
   ├─ Models/
   │  ├─ DbObjectType, DbObject, TableColumn, DatabaseSchema
   │  ├─ CompareStatus, ObjectComparison
   │  ├─ ConnectionInfo, LastConnection
   │  └─ ServerProfile, ComparisonParameters, AppSettings
   └─ Services/
      ├─ SchemaExtractor.cs       extrai schema; ListDatabasesAsync; TestConnectionAsync
      ├─ SqlNormalizer.cs         normaliza DDL + hash SHA-256
      ├─ SchemaComparer.cs        classifica objetos; respeita ConsiderColumnOrder
      ├─ DiffEngine.cs            diff lado-a-lado (DiffPlex)
      ├─ SyncScriptGenerator.cs   gera script de sync (ordem por dependência, GO)
      ├─ TableAlterGenerator.cs   ALTER TABLE coluna-a-coluna (ADD/DROP/ALTER/DEFAULT)
      ├─ ScriptExecutor.cs        executa por batches (split GO) em 1 transação
      └─ SettingsStore.cs         persiste settings em JSON (senha via DPAPI)
```

> **Importante (csproj):** o projeto da UI fica na raiz e faz glob recursivo de
> `**/*.cs`. Por isso `SQLStructDiff.csproj` tem `<Compile Remove="SQLStructDiff.Core\**\*.cs" />`
> (e Content/None/EmbeddedResource) para não engolir os fontes/obj do Core.

## 4. Pacotes NuGet

- `Microsoft.Data.SqlClient` (Core) — acesso ao SQL Server
- `DiffPlex` (Core e UI) — diff
- `System.Security.Cryptography.ProtectedData` (Core) — DPAPI p/ senha
  → por isso o Core é `net9.0-windows` (DPAPI é Windows-only).

## 5. Como buildar / rodar

```bash
dotnet build SQLStructDiff.sln      # deve dar 0 avisos / 0 erros
dotnet run --project SQLStructDiff.csproj   # precisa de Windows + SQL Server real
```

A UI precisa de ambiente gráfico Windows e bancos SQL Server reais para testar.

## 6. Como funciona cada parte (resumo técnico)

- **Extração:** `SchemaExtractor.ExtractAsync` retorna `DatabaseSchema` com `DbObject`s.
  Cada `DbObject` tem `Definition` (normalizada, p/ hash/diff), `RawDefinition`
  (DDL executável original) e, para tabelas, `Columns` (List<TableColumn>).
- **Chave de comparação:** `DbObject.Key = "{Type}:[{Schema}].[{Name}]"`. Índices
  usam `Name = "Tabela.Indice"` para casar entre bancos.
- **Comparação:** hash igual ⇒ Equal; senão Different / OnlyInA / OnlyInB.
  Se `ConsiderColumnOrder == false`, tabelas que diferem só na ordem das colunas
  são tratadas como Equal (assinatura com colunas ordenadas).
- **Script de sync:** `SET XACT_ABORT ON` + `BEGIN TRAN` + batches separados por `GO`
  + `COMMIT`. Ordem: DROP (trigger→proc→view→idx→tbl), depois CREATE/ALTER
  (tbl→idx→view→proc→trigger). Views/Procs/Triggers alterados usam `CREATE OR ALTER`.
  Índices alterados: DROP+CREATE.
  **Tabelas que existem nos 2 bancos e diferem → `TableAlterGenerator`** gera
  ALTER COLUMN / ADD / DROP COLUMN + ajuste de DEFAULT (constraint).
- **Execução:** `ScriptExecutor` separa por `GO`, ignora o controle de transação do
  script e roda tudo em UMA transação própria, com rollback em erro.
- **Settings:** `%APPDATA%\SQLStructDiff\settings.json`. Senhas criptografadas com
  DPAPI (CurrentUser). Guarda perfis de servidor, os parâmetros de comparação e a
  **última conexão** de cada painel (A/B) para restaurar ao reabrir o app.
- **Direção do script (`ComparisonParameters.Bidirectional`):** quando `true` (padrão)
  gera nos dois sentidos (A→B e B→A); quando `false` (unidirecional) só da esquerda (A)
  para a direita (B) — a MainForm oculta os botões/menu B→A via `ApplyDirectionMode()`.
- **Nomes reais dos bancos:** títulos, cabeçalhos do grid, botões, tooltips e
  cabeçalho do `.sql` usam `ConnectionInfo.DisplayName`/`FullDisplayName` (database e
  servidor), não mais "A"/"B" genéricos.
- **Restaurar sessão:** `ConnectionForm.Load` chama `ConnectionPanel.RestoreAsync`
  (reconecta e reseleciona o database); `FormClosing` chama `CaptureConnection` e salva.
- **Scripts de view/procedure sem marcadores:** `SyncScriptGenerator.IsModule(type)`
  suprime as linhas `-- CREATE/ALTER/DROP ...` para views e procedures (tabelas/índices
  mantêm os marcadores).
- **Toolbar da MainForm:** botões só de ícone (fonte *Segoe MDL2 Assets*) com tooltips;
  inclui geração de script "tudo" e "somente selecionados", nos dois sentidos.

## 7. Estado atual — FEITO

- [x] Extração de Tabelas/Views/Procedures/Índices/**Triggers** (sem data de geração)
- [x] Ícone da aplicação (`appicon.ico` + `ApplicationIcon`; janelas usam o ícone do exe)
- [x] Comparação + status por objeto
- [x] Diff visual lado-a-lado (WinMerge-like) no DataGridView
- [x] Geração de script A→B e B→A (completo) e **por objeto** (menu de contexto)
- [x] Execução do script no alvo (transação + confirmação)
- [x] Salvar perfis de servidor (JSON + senha DPAPI)
- [x] Database selecionável via combo (conexão validada ao listar)
- [x] Tela de parâmetros + persistência (1ª opção: "Considerar ordem dos campos")
- [x] **ALTER TABLE coluna-a-coluna** para tabelas existentes nos 2 bancos
      (ADD/DROP/ALTER COLUMN + DROP/ADD default constraint)
- [x] UI usa os **nomes reais** dos bancos (database/servidor) em vez de "A"/"B"
- [x] Indicador visível de execução (overlay central + barra *marquee*) ao comparar
- [x] Toolbar com **botões de ícone + tooltips**; scripts "tudo" e "somente selecionados"
- [x] Parâmetro **bidirecional × unidirecional** (só A→B) para geração de script
- [x] **Restaurar a última conexão** (A e B) ao reabrir o app
- [x] Scripts de view/procedure **sem os comentários de marcação** da ferramenta
- [x] Botões de perfil (salvar/excluir) como **ícones** com tooltip

## 8. O que falta / próximos passos sugeridos

- [ ] **Projeto de testes** (`SQLStructDiff.Tests`) — cobrir SqlNormalizer,
      SchemaComparer (inclusive ConsiderColumnOrder) e TableAlterGenerator.
- [ ] ALTER de IDENTITY e de PRIMARY KEY (hoje só emite comentário de atenção).
- [ ] Comparar/sincronizar Foreign Keys e constraints CHECK (ainda não extraídas).
- [ ] Diff visual com realce intra-linha (hoje é por linha inteira).
- [ ] Filtro por schema na UI; performance/cancelamento em bancos grandes.
- [ ] Mais parâmetros: ignorar collation, ignorar espaços em módulos, etc.
- [x] Telas convertidas para o padrão Designer do Visual Studio.

## 9. Convenções do projeto

- Clean Code, sem duplicação, async/await, código legível para júnior.
- Respostas/decisões em português.
- Comentários explicam o "porquê", não o óbvio.

## 10. Limitações conscientes

- Geração de ALTER de tabela cobre coluna/tipo/nulabilidade/default. Mudança de
  IDENTITY ou PK não é gerada automaticamente (emite comentário `-- ATENÇÃO`).
- FKs e CHECK constraints ainda não fazem parte da comparação.
- `SqlNormalizer` colapsa espaços para hash/diff; por isso o script executável usa
  sempre `RawDefinition`, nunca a `Definition` normalizada.
