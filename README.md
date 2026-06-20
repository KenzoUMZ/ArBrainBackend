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

- Cervejas: ArBrain IPA, Golden Lager (com parâmetros)
- Tanques: FV-01 (1000L), FV-02 (1500L)
- Lote **IPA001** com 2 apontamentos de exemplo
