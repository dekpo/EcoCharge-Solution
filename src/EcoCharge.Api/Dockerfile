# syntax=docker/dockerfile:1
#
# Multi-stage build: the .NET SDK is only needed to *build* the image.
# The final runtime image ships the much smaller ASP.NET runtime, and no
# local .NET installation is ever required to build or run this project.

# ---- Build stage -----------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY EcoCharge.slnx .
COPY global.json .
COPY src/EcoCharge.Domain/EcoCharge.Domain.csproj src/EcoCharge.Domain/
COPY src/EcoCharge.Application/EcoCharge.Application.csproj src/EcoCharge.Application/
COPY src/EcoCharge.Infrastructure/EcoCharge.Infrastructure.csproj src/EcoCharge.Infrastructure/
COPY src/EcoCharge.Api/EcoCharge.Api.csproj src/EcoCharge.Api/
COPY tests/EcoCharge.Tests/EcoCharge.Tests.csproj tests/EcoCharge.Tests/

# Inject host CA certificates if present (needed when network uses TLS inspection).
# Generate certs/win-cas.crt once — see certs/README.md.
COPY certs/ /tmp/certs/
RUN if ls /tmp/certs/*.crt > /dev/null 2>&1; then \
      cp /tmp/certs/*.crt /usr/local/share/ca-certificates/ && \
      update-ca-certificates; \
    fi

RUN dotnet restore src/EcoCharge.Api/EcoCharge.Api.csproj

COPY src/ src/
RUN dotnet publish src/EcoCharge.Api/EcoCharge.Api.csproj -c Release -o /app --no-restore

# ---- Runtime stage -----------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN useradd --system --no-create-home --user-group ecocharge \
    && mkdir -p /data && chown ecocharge:ecocharge /data
USER ecocharge

COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "EcoCharge.Api.dll"]
