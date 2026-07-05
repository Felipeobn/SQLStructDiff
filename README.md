# SQLStructDiff

Aplicação **desktop C# WinForms (.NET 9, Windows)** que compara a **estrutura de dois
bancos SQL Server** e gera scripts de sincronização DDL.

Compara quatro tipos de objeto: **Tabelas, Views, Procedures e Índices**.

> Fluxo: conectar nos 2 bancos → extrair schema → comparar → ver as diferenças em um
> diff visual lado a lado (estilo WinMerge) → gerar/executar o script de sincronização.

---

## Recursos

- **Comparação de schema** de Tabelas, Views, Procedures e Índices, com status por
  objeto: igual, diferente, só no banco da esquerda ou só no da direita.
- **Diff visual lado a lado** com realce por linha (inserido / removido / alterado).
- **Geração de script de sincronização** DDL, com opção de:
  - **só gerar** (copiar para a área de transferência ou salvar `.sql`); ou
  - **executar no banco alvo** — sempre em **uma transação com rollback** em caso de erro.
- Gerar script de **tudo** ou **somente do objeto selecionado**, nos dois sentidos.
- **Direção configurável**: bidirecional (A↔B) ou unidirecional (apenas da esquerda
  para a direita).
- **Perfis de servidor** salvos (com senha protegida por DPAPI) e **database** escolhido
  em combo (a conexão é validada ao listar os bancos).
- **Restaura a última conexão** usada ao reabrir o app.
- Nomes reais dos bancos (database/servidor) na interface — não "A/B" genéricos.
- Indicador visível enquanto compara os schemas.

---

## Requisitos

- **Windows** (a UI é WinForms e a criptografia de senha usa DPAPI, que é Windows-only).
- **.NET SDK 9** ([download](https://dotnet.microsoft.com/download)).
- Acesso a uma ou mais instâncias de **SQL Server** para comparar de fato.

---

## Como buildar e rodar

```bash
dotnet build SQLStructDiff.sln          # deve compilar com 0 avisos / 0 erros
dotnet run --project SQLStructDiff.csproj
```

> A UI precisa de ambiente gráfico Windows e de bancos SQL Server reais para uso.

---

## Como usar

1. Na **tela de conexão**, configure os dois bancos (esquerda e direita):
   - digite o **servidor**, escolha **Autenticação Windows** ou informe usuário/senha;
   - clique em **Conectar** e selecione o **database** no combo;
   - opcionalmente **salve** o servidor como um perfil (ícone 💾) para reusar depois.
2. Ajuste os **Parâmetros** (opcional): considerar a ordem das colunas; permitir
   geração de script bidirecional ou unidirecional.
3. Clique em **Comparar**. A tela principal mostra a árvore de objetos e o diff.
4. Selecione um objeto para ver o **diff lado a lado**.
5. Gere o script pela **toolbar** (tudo ou selecionados, no sentido desejado) ou pelo
   **menu de contexto** (botão direito na árvore). No preview você pode **copiar**,
   **salvar** ou **executar** no banco alvo.

A próxima vez que abrir o app, ele **restaura os dados de conexão** usados por último.

---

## Onde ficam as configurações

`%APPDATA%\SQLStructDiff\settings.json`

Guarda os perfis de servidor, os parâmetros de comparação e a última conexão de cada
lado. **As senhas são criptografadas com DPAPI** (por usuário do Windows) — o arquivo
não contém senhas em texto puro.

---

## Estrutura da solução

```
SQLStructDiff.sln
├─ SQLStructDiff.csproj      (raiz) camada UI — WinForms, padrão Designer (X.cs + X.Designer.cs)
│  ├─ Program.cs             inicia a ConnectionForm
│  └─ UI/                    ConnectionForm, ConnectionPanel, MainForm,
│                            ScriptPreviewForm, ParametersForm, Prompt
└─ SQLStructDiff.Core/       biblioteca de lógica (net9.0-windows)
   ├─ Models/                schema, comparação, conexão, settings
   └─ Services/              SchemaExtractor, SqlNormalizer, SchemaComparer,
                             DiffEngine, SyncScriptGenerator, TableAlterGenerator,
                             ScriptExecutor, SettingsStore
```

Pacotes: `Microsoft.Data.SqlClient`, `DiffPlex`, `System.Security.Cryptography.ProtectedData`.

Para detalhes de arquitetura e decisões de projeto, veja [PROJECT.md](PROJECT.md).

---

## Limitações conscientes

- A geração de `ALTER TABLE` cobre coluna/tipo/nulabilidade/default; **mudança de
  IDENTITY ou de PRIMARY KEY** não é gerada automaticamente (emite comentário de atenção).
- **Foreign Keys** e **constraints CHECK** ainda não fazem parte da comparação.
- Apenas **SQL Server** (sem Postgres/MySQL por ora).
- O diff é por linha inteira (sem realce intra-linha).

---

## Licença

Distribuído sob a licença **MIT** — veja [LICENSE](LICENSE).
