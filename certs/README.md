# certs/ — Local CA certificates for Docker builds

**Purpose**: If your network uses TLS inspection (antivirus, corporate proxy,
router-level HTTPS scanning), Docker containers cannot reach `nuget.org` or
other HTTPS sources without trusting the network's custom root CA.

This directory is intended to hold a `win-cas.crt` file (PEM format) that the
API and client Dockerfiles inject into the container trust store before
`dotnet restore` / `npm ci`.

**If Docker Desktop itself fails TLS** (e.g. `x509: certificate signed by
unknown authority` when pulling `docker.io/docker/dockerfile:1`), that is
host antivirus / HTTPS inspection — not something a Dockerfile can fix.
Exclude Docker Hub, NuGet, and npmjs from HTTPS scanning, then retry.

**The `.crt` files are gitignored** — they never leave your machine.

## One-time setup (run once per machine)

Open **PowerShell** (this is the one case where PS is genuinely simpler than cmd
for certificate export — it is a one-time admin operation):

```powershell
Get-ChildItem -Path Cert:\LocalMachine\Root, Cert:\CurrentUser\Root |
  Sort-Object -Unique Thumbprint |
  ForEach-Object {
    "-----BEGIN CERTIFICATE-----"
    [Convert]::ToBase64String($_.RawData, 'InsertLineBreaks')
    "-----END CERTIFICATE-----"
  } | Set-Content -Encoding ascii certs\win-cas.crt
```

Then run `docker compose up --build` normally. The Dockerfile will pick up
`win-cas.crt` automatically.

## After generating the file

```
EcoCharge-Solution/
└── certs/
    ├── README.md    ← this file (tracked by git)
    └── win-cas.crt  ← your CA export (gitignored, local only)
```
