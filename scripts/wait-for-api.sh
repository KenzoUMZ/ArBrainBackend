#!/usr/bin/env bash
set -euo pipefail

HOST="${API_HOST:-127.0.0.1}"
PORT="${API_PORT:-5242}"
MAX_WAIT="${API_WAIT_SECONDS:-120}"
URL="http://${HOST}:${PORT}/api/health"

echo "==> Aguardando API em ${URL} (até ${MAX_WAIT}s)..."

for ((i = 1; i <= MAX_WAIT; i++)); do
  if curl -sf "$URL" >/dev/null 2>&1; then
    echo "==> API pronta."
    exit 0
  fi

  if (( i % 5 == 0 )); then
    echo "  ainda aguardando... (${i}/${MAX_WAIT}s)"
  fi

  sleep 1
done

echo ""
echo "AVISO: a API não respondeu em ${MAX_WAIT}s."
echo "O frontend pode exibir erro 502 até você subir a API (dotnet run --project src/ArBrain.Api)."
exit 0
