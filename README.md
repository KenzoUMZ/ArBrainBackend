# ArBrain Backend

API .NET com **Clean Architecture** para gestão fermentativa de cervejas.

## Estrutura

```text
src/
├── ArBrain.Domain/           Entidades, enums e regras de negócio
├── ArBrain.Application/      DTOs, services, contratos
├── ArBrain.Infrastructure/   EF Core + PostgreSQL
└── ArBrain.Api/              Endpoints REST
```

## Rodar

**Pré-requisito:** [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado e **em execução** (ícone da baleia ativo na barra de menu).

### Opção recomendada — Run and Debug (VS Code / Cursor)

O projeto inclui configurações em [`.vscode/launch.json`](.vscode/launch.json) e tarefas em [`.vscode/tasks.json`](.vscode/tasks.json).

**Abra o workspace multi-root** [`ArBrain.code-workspace`](ArBrain.code-workspace) (backend + frontend juntos) ou abra a pasta `ArBrainBackend` com o frontend em `../ArBrainFrontend`.

No painel **Run and Debug** (`⇧⌘D` / `Ctrl+Shift+D`), escolha uma das duas opções:

| Configuração | O que faz |
|--------------|-----------|
| **Fullstack (API + Frontend)** | Sobe o banco, builda e inicia a API, aguarda o health check e inicia o Vite. Mantém os dados existentes no banco. |
| **Fullstack + Seed (reset DB)** | Igual ao anterior, mas **esvazia o banco** e popula com dados de desenvolvimento (20 cervejas, 20 tanques, 20 lotes). |

Pressione **F5** (ou o botão de play) para iniciar. Ao parar o debug, ambos os processos são encerrados (`stopAll: true`).

**URLs após subir:**

| Serviço | URL |
|---------|-----|
| Frontend | http://localhost:5173 |
| API | http://localhost:5242 |
| Health check | http://localhost:5242/api/health |

#### O que cada run executa (por trás)

**Fullstack (API + Frontend)**

1. `db: start` — roda [`scripts/start-db.sh`](scripts/start-db.sh): verifica/abre o Docker e sobe o PostgreSQL (`docker compose up -d`)
2. `api: free-port` — libera a porta `5242` se já estiver em uso
3. `build` — compila `ArBrain.Api`
4. **ArBrain API** — inicia a API em `http://localhost:5242` (seed mínimo só se o banco estiver vazio)
5. `frontend: free-port` — libera a porta `5173`
6. `api: wait-for-health` — aguarda `GET /api/health` responder (até 120s)
7. **Frontend (Vite)** — `npm run dev` em `../ArBrainFrontend` e abre o navegador quando o Vite estiver pronto

**Fullstack + Seed (reset DB)**

Mesmo fluxo acima, mas a API recebe `ARBRAIN_SEED_MODE=reset`. Na subida, a API:

1. Aplica migrations pendentes
2. Executa `TRUNCATE` nas tabelas de dados
3. Popula o banco com o seed de desenvolvimento (ver [Seed de demonstração](#seed-de-demonstração))

> Use **Fullstack + Seed** quando quiser testar paginação, filtros e ordenação com volume de dados.

#### Rodar só a API ou só o frontend

As configurações individuais (`ArBrain API`, `ArBrain API (Seed)`, `Frontend`) existem para os compounds funcionarem e ficam ocultas no seletor. Para subir apenas um lado, use o terminal:

```bash
# Só API (seed mínimo se banco vazio)
dotnet run --project src/ArBrain.Api

# Só API com reset + seed de desenvolvimento
ARBRAIN_SEED_MODE=reset dotnet run --project src/ArBrain.Api

# Só frontend (API já deve estar rodando)
cd ../ArBrainFrontend && npm run dev
```

### Opção manual — terminal

```bash
# Opção A — script que abre o Docker e sobe o banco automaticamente
./scripts/start-db.sh

# Opção B — manual
open -a Docker          # aguarde ficar "Running" (~30s)
docker compose up -d
docker compose ps       # deve mostrar arbrain-postgres healthy

# Subir a API (em outro terminal ou depois do banco)
dotnet run --project src/ArBrain.Api
```

> **Importante:** rode um comando por vez. Não cole linhas com `#` no terminal (zsh interpreta como comando).
> Só rode `dotnet run` **depois** que `docker compose ps` mostrar o container saudável.

> Se o banco já existia com o schema antigo (estoque), recrie o volume:
> `docker compose down -v && docker compose up -d`

### Problemas comuns

| Erro | Solução |
|------|---------|
| `failed to connect to the docker API ... docker.sock` | Abra o **Docker Desktop** e aguarde iniciar |
| `Connection refused` em `localhost:5432` | Rode `docker compose up -d` antes da API |
| `docker compose ps` vazio ou container exited | `docker compose logs db` e depois `docker compose up -d` |

## Testar com Postman

Arquivos na pasta [`postman/`](postman/):

| Arquivo | Descrição |
|---------|-----------|
| [`ArBrain-API.postman_collection.json`](postman/ArBrain-API.postman_collection.json) | Todos os endpoints + fluxo de teste automatizado |
| [`ArBrain-Local.postman_environment.json`](postman/ArBrain-Local.postman_environment.json) | Variáveis locais (`baseUrl`, `beerId`, `tankId`, etc.) |

### Importar no Postman

1. Abra o Postman → **Import** → selecione os dois arquivos da pasta `postman/`
2. Ative o environment **ArBrain - Local**
3. Execute a pasta **00 - Fluxo de teste** com **Run collection** (ordem 1 → 10)

O fluxo valida health check, listagens do seed, cadastros (cerveja, parâmetros, tanque, apontamento), dashboard e histórico de lotes. Os scripts salvam `beerId` e `tankId` automaticamente entre requests.

### Teste rápido via curl

```bash
curl http://localhost:5242/api/health
curl http://localhost:5242/api/dashboard
curl http://localhost:5242/api/fermentation-records/batches/IPA001
```

## Endpoints

### Cervejas
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/beers` | Listar cervejas |
| GET | `/api/beers/{id}` | Detalhe |
| POST | `/api/beers` | Cadastrar (nome, estilo) |
| PUT | `/api/beers/{id}` | Atualizar |
| DELETE | `/api/beers/{id}` | Inativar |
| PUT | `/api/beers/{id}/parameters` | Parâmetros fermentativos |

### Tanques
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/tanks` | Listar tanques |
| POST | `/api/tanks` | Cadastrar (nome, capacidade) |
| PUT | `/api/tanks/{id}` | Atualizar |
| DELETE | `/api/tanks/{id}` | Inativar |

### Apontamentos fermentativos
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/fermentation-records` | Listar apontamentos |
| POST | `/api/fermentation-records` | Registrar apontamento |
| GET | `/api/fermentation-records/batches` | Resumo de lotes |
| GET | `/api/fermentation-records/batches/{lote}` | Histórico do lote |

### Dashboard
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/dashboard` | Indicadores de conformidade |

## Classificação de conformidade

Cada apontamento é classificado automaticamente:

- **WithinStandard** — todos os indicadores dentro da faixa
- **RequiresAttention** — dentro da faixa, porém próximo aos limites (10%)
- **OutOfStandard** — pelo menos um indicador fora da faixa

## Seed de demonstração

O seed roda automaticamente na subida da API (`Program.cs` → `DbInitializer`). O comportamento depende de `ARBRAIN_SEED_MODE`:

| Modo | Quando | Comportamento |
|------|--------|---------------|
| *(padrão)* | **Fullstack** ou `dotnet run` sem variável | Aplica migrations. Se o banco estiver **vazio**, insere o seed mínimo. Se já houver dados, **não altera nada**. |
| `reset` | **Fullstack + Seed** ou `ARBRAIN_SEED_MODE=reset dotnet run` | Aplica migrations, **esvazia** as tabelas e insere o seed de desenvolvimento. |

### Seed mínimo (banco vazio)

- **2 cervejas:** ArBrain IPA, Golden Lager (com parâmetros fermentativos)
- **2 tanques:** Tanque FV-01 (1000 L), Tanque FV-02 (1500 L)
- **2 lotes:** IPA001 (2 apontamentos), LAG001 (1 apontamento)

### Seed de desenvolvimento (`ARBRAIN_SEED_MODE=reset`)

Pensado para testar paginação, busca, filtros de conformidade e histórico de lotes no frontend:

- **20 cervejas** — variedade de estilos (IPA, Lager, Pilsner, Stout, Porter, Weiss, Sour, Pale Ale)
- **20 tanques** — Tanque FV-01 a FV-20 (capacidades de 900 L a 2800 L)
- **20 lotes** — um lote por cerveja (ex.: IPA001, LAG001, PIL001…)
- **~140 apontamentos** — 6 a 8 medições por lote, com evolução temporal e mix de status de conformidade

> **Atenção:** o modo `reset` apaga todos os dados de cervejas, tanques e apontamentos. Use apenas em ambiente local de desenvolvimento.
