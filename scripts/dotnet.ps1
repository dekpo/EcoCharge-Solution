<#
.SYNOPSIS
  Runs any `dotnet` CLI command inside the official .NET 10 SDK Docker image,
  with the repository root mounted as the working directory.

  Zero local .NET SDK install required. Docker Desktop must be running.

.EXAMPLE
  ./scripts/dotnet.ps1 --version
  ./scripts/dotnet.ps1 build EcoCharge.slnx
  ./scripts/dotnet.ps1 test
  ./scripts/dotnet.ps1 ef migrations add InitialCreate -p src/EcoCharge.Infrastructure -s src/EcoCharge.Api
#>

$RepoRoot = Split-Path -Parent $PSScriptRoot
$SdkImage = "mcr.microsoft.com/dotnet/sdk:10.0"

docker run --rm -it `
  -v "${RepoRoot}:/src" `
  -w /src `
  -e DOTNET_CLI_TELEMETRY_OPTOUT=1 `
  -e NUGET_XMLDOC_MODE=skip `
  $SdkImage dotnet @args
