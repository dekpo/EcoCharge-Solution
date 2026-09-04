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

# Local tools (dotnet-ef) are restored in the same container as the command,
# because each invocation is ephemeral.
$quotedArgs = ($args | ForEach-Object {
    if ($_ -match "[\s""']") { '"' + ($_ -replace '"', '\"') + '"' } else { $_ }
}) -join ' '

docker run --rm -it `
  -v "${RepoRoot}:/src" `
  -w /src `
  -e DOTNET_CLI_TELEMETRY_OPTOUT=1 `
  -e NUGET_XMLDOC_MODE=skip `
  $SdkImage `
  bash -c "if [ -f .config/dotnet-tools.json ] || [ -f dotnet-tools.json ]; then dotnet tool restore; fi && dotnet $quotedArgs"
