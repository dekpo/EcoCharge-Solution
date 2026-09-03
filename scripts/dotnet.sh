#!/usr/bin/env bash
# Runs any `dotnet` CLI command inside the official .NET 10 SDK Docker image,
# with the repository root mounted as the working directory.
#
# Zero local .NET SDK install required. Docker (daemon) must be running.
#
# Usage:
#   ./scripts/dotnet.sh --version
#   ./scripts/dotnet.sh build EcoCharge.sln
#   ./scripts/dotnet.sh test
#   ./scripts/dotnet.sh ef migrations add InitialCreate -p src/EcoCharge.Infrastructure -s src/EcoCharge.Api

set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SDK_IMAGE="mcr.microsoft.com/dotnet/sdk:10.0"

docker run --rm -it \
  -v "${REPO_ROOT}:/src" \
  -w /src \
  -e DOTNET_CLI_TELEMETRY_OPTOUT=1 \
  -e NUGET_XMLDOC_MODE=skip \
  "${SDK_IMAGE}" dotnet "$@"
