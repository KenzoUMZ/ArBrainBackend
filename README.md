# ArBrain Backend

API .NET com **Clean Architecture** para o ERP de gestão de cervejas.

## Estrutura

```text
src/
├── ArBrain.Api/              # HTTP, endpoints, DI, CORS
├── ArBrain.Application/      # DTOs, services, interfaces
├── ArBrain.Domain/           # Entidades e enums
└── ArBrain.Infrastructure/   # EF Core, repositórios, migrations
docker-compose.yml            # PostgreSQL local
ArBrain.slnx                  # Solution
```

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

## Subir o banco

```bash
docker compose up -d
```

## Rodar a API

```bash
dotnet restore
dotnet run --project src/ArBrain.Api
```

Endpoints:

- `GET /api/health`
- `GET /api/beers`

## Migrations

```bash
dotnet tool restore
dotnet ef migrations add NomeDaMigration \
  --project src/ArBrain.Infrastructure \
  --startup-project src/ArBrain.Api \
  --output-dir Migrations
```

A API aplica migrations e seed automaticamente no startup.

## Camadas

| Projeto | Responsabilidade |
|---------|------------------|
| **Domain** | Regras e entidades puras |
| **Application** | Casos de uso, DTOs, contratos |
| **Infrastructure** | Persistência (PostgreSQL + EF Core) |
| **Api** | Exposição HTTP |
