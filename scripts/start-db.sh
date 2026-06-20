#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

echo "==> Verificando Docker..."

if ! command -v docker >/dev/null 2>&1; then
  echo "Docker CLI não encontrado. Instale o Docker Desktop:"
  echo "https://www.docker.com/products/docker-desktop/"
  exit 1
fi

if ! docker info >/dev/null 2>&1; then
  echo "Docker Desktop não está rodando."
  echo "Abrindo Docker Desktop... (aguarde até ficar 'Running')"
  open -a Docker || true

  for i in {1..30}; do
    if docker info >/dev/null 2>&1; then
      echo "Docker pronto."
      break
    fi
    echo "  aguardando Docker... ($i/30)"
    sleep 2
  done
fi

if ! docker info >/dev/null 2>&1; then
  echo ""
  echo "Não foi possível conectar ao Docker."
  echo "Abra manualmente: Applications > Docker"
  echo "Depois rode novamente: ./scripts/start-db.sh"
  exit 1
fi

echo "==> Subindo PostgreSQL..."
docker compose up -d

echo "==> Status do container:"
docker compose ps

echo ""
echo "Banco pronto. Agora rode:"
echo "  dotnet run --project src/ArBrain.Api"
