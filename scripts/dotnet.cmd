@echo off
REM Runs any `dotnet` CLI command inside the official .NET 10 SDK Docker image,
REM with the repository root mounted as the working directory.
REM
REM Zero local .NET SDK install required. Docker Desktop must be running.
REM This is the default Windows wrapper (classic cmd). Prefer this over
REM scripts\dotnet.ps1.
REM
REM Usage (from repo root, in cmd.exe):
REM   scripts\dotnet.cmd --version
REM   scripts\dotnet.cmd restore
REM   scripts\dotnet.cmd build EcoCharge.slnx
REM   scripts\dotnet.cmd test
REM   scripts\dotnet.cmd run --project src/EcoCharge.Api

setlocal
set "REPO_ROOT=%~dp0.."
set "SDK_IMAGE=mcr.microsoft.com/dotnet/sdk:10.0"

docker run --rm -it ^
  -v "%REPO_ROOT%:/src" ^
  -w /src ^
  -e DOTNET_CLI_TELEMETRY_OPTOUT=1 ^
  -e NUGET_XMLDOC_MODE=skip ^
  %SDK_IMAGE% dotnet %*
