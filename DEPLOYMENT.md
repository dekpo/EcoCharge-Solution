# Deployment

This guide describes how to run EcoCharge on **any Linux host that can run
Docker** (a personal VPS, a homelab box, or a spare machine). The default
demo path is still `docker compose up --build` on your laptop — this file
is optional. It contains no hostnames, IP addresses, or secrets.

**Deployment is never automatic.** Shipping a new version to a production
host always requires an explicit, per-action decision from the project
owner. This document only describes *how*; it does not perform any remote
action.

## What you are deploying

| Piece | Default local/dev ports | Role |
|---|---|---|
| API (`ecocharge-api`) | host `5080` → container `8080` | ASP.NET Core Web API, EF Core, charge-simulation worker |
| Client (`ecocharge-client`) | host `5173` → container `80` | React SPA served by nginx |
| SQLite file | container path `/data/ecocharge.db` | Default database (volume-backed) |
| PostgreSQL (optional) | host `5432` | Prod-like database, Compose profile `postgres` |

Health check (API): `GET /health` → `{ "status": "healthy" }`.

The SPA calls the API **from the browser**. The API public URL is baked into
the client image at **build time** (`VITE_API_BASE_URL`). Changing the public
API URL requires rebuilding the client image. CORS on the API must allow the
public origin of the SPA.

## Prerequisites

- Docker Engine + Docker Compose plugin (or Docker Desktop) on the host
- Git, to clone this repository
- A reverse proxy with TLS is strongly recommended in production (see
  [TLS and a reverse proxy](#tls-and-a-reverse-proxy))
- No local .NET SDK or Node.js install is required to *run* the stack

## Configuration

Do **not** commit real connection strings, passwords, or hostnames. Use a
host-local `.env` next to `docker-compose.yml` (gitignored) or the process
environment.

| Variable | Used by | Purpose | Example (not for production) |
|---|---|---|---|
| `ECOCHARGE_CONNECTION_STRING` | API | EF Core connection string | `Data Source=/data/ecocharge.db` |
| `ECOCHARGE_CLIENT_ORIGIN` | API | Allowed CORS origin (the public SPA URL) | `https://app.example.com` |
| `ECOCHARGE_API_BASE_URL` | Client **build arg** | Public API URL the browser will call | `https://api.example.com` |
| `ECOCHARGE_DB_PASSWORD` | Postgres profile only | `POSTGRES_PASSWORD` | a strong secret |

Related ASP.NET environment variables (Compose already maps the first two):

- `ConnectionStrings__Default` — same as `ECOCHARGE_CONNECTION_STRING`
- `Cors__AllowedOrigin` — same as `ECOCHARGE_CLIENT_ORIGIN`
- `ConnectionStrings__Provider` — `Sqlite` (default) or `Postgres`
- `ASPNETCORE_ENVIRONMENT` — Compose sets `Production`

If the SPA and the API are served under **different public origins**, set
`ECOCHARGE_CLIENT_ORIGIN` to the SPA origin and `ECOCHARGE_API_BASE_URL` to
the API origin, then rebuild the client.

## Database and migrations

On API startup the host applies pending EF Core migrations
(`Database.MigrateAsync()`). The initial schema lives in
`src/EcoCharge.Infrastructure/Persistence/Migrations` and is generated
against **SQLite**, which is the default provider.

If you previously ran a build that used `EnsureCreated` (no migrations
history table), delete the SQLite file or the Docker volume **once** before
the first start with migrations. Otherwise startup fails because the tables
already exist without `__EFMigrationsHistory`.

```bash
docker compose down
docker volume ls          # look for the project volume, typically ecocharge_ecocharge-data
docker volume rm <volume-name>
docker compose up --build
```

PostgreSQL is supported via `ConnectionStrings__Provider=Postgres` and the
Compose `postgres` profile. The checked-in migrations target SQLite; do not
point a production API at PostgreSQL until a PostgreSQL-specific migration
set has been added.

To add a new migration later (Docker SDK wrapper, no native SDK):

```bat
REM Windows — cmd.exe
scripts\dotnet.cmd ef migrations add <Name> --project src/EcoCharge.Infrastructure --startup-project src/EcoCharge.Api --output-dir Persistence/Migrations
```

```bash
# macOS / Linux / WSL
./scripts/dotnet.sh ef migrations add <Name> --project src/EcoCharge.Infrastructure --startup-project src/EcoCharge.Api --output-dir Persistence/Migrations
```

## Option A — Docker Compose (recommended)

From the repository root on the target host:

```bash
git clone <this-repository-url> EcoCharge-Solution
cd EcoCharge-Solution
```

Create a `.env` file (never commit it):

```bash
ECOCHARGE_CLIENT_ORIGIN=https://app.example.com
ECOCHARGE_API_BASE_URL=https://api.example.com
# Optional: override SQLite location or switch providers later
# ECOCHARGE_CONNECTION_STRING=Data Source=/data/ecocharge.db
```

Then:

```bash
docker compose up --build -d
```

- API: `http://<host>:5080` (or whatever you publish)
- Client: `http://<host>:5173`

SQLite data is stored in the named volume `ecocharge-data`, mounted at
`/data` in the API container. Back it up with a volume backup or by copying
`/data/ecocharge.db` while the API is stopped.

Optional PostgreSQL sidecar (development / prod-like testing only):

```bash
docker compose --profile postgres up --build -d
```

That starts Postgres; the API still uses SQLite unless you also set
`ConnectionStrings__Provider=Postgres` and a Postgres
`ConnectionStrings__Default` (and have PostgreSQL migrations). Do not
publish port `5432` on a public interface.

### Updating

```bash
cd EcoCharge-Solution
git pull
docker compose up --build -d
```

Pending migrations run when the new API container starts.

### Stopping

```bash
docker compose down          # keeps volumes
docker compose down -v       # also deletes database volumes — destructive
```

## Option B — systemd + Nginx

Use this when you want the Compose stack to start on boot and to terminate
TLS / virtual hosts at Nginx on the host (or another reverse proxy).

### systemd unit (starts Compose)

Example unit file (adjust `WorkingDirectory` to where you cloned the repo):

```ini
[Unit]
Description=EcoCharge Docker Compose stack
Requires=docker.service
After=docker.service

[Service]
Type=oneshot
RemainAfterExit=yes
WorkingDirectory=/opt/ecocharge
ExecStart=/usr/bin/docker compose up -d
ExecStop=/usr/bin/docker compose down
TimeoutStartSec=0

[Install]
WantedBy=multi-user.target
```

```bash
sudo systemctl daemon-reload
sudo systemctl enable --now ecocharge.service
```

Keep `.env` in that working directory so Compose picks it up.

### Nginx as reverse proxy

Terminate TLS at Nginx and proxy to the locally bound Compose ports. The
browser must see the **same** public API URL that was baked into the client
image (`ECOCHARGE_API_BASE_URL`).

Sketch (replace server names with yours; obtain certificates separately):

```nginx
# SPA
server {
    listen 443 ssl http2;
    server_name app.example.com;
    # ssl_certificate     /etc/letsencrypt/live/app.example.com/fullchain.pem;
    # ssl_certificate_key /etc/letsencrypt/live/app.example.com/privkey.pem;

    location / {
        proxy_pass http://127.0.0.1:5173;
        proxy_set_header Host $host;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}

# API
server {
    listen 443 ssl http2;
    server_name api.example.com;
    # ssl_certificate     /etc/letsencrypt/live/api.example.com/fullchain.pem;
    # ssl_certificate_key /etc/letsencrypt/live/api.example.com/privkey.pem;

    location / {
        proxy_pass http://127.0.0.1:5080;
        proxy_set_header Host $host;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

Bind Compose ports to localhost only if Nginx is the public entrypoint, for
example `127.0.0.1:5080:8080` and `127.0.0.1:5173:80` in an override file
(`docker-compose.override.yml` is gitignored).

A single-host layout (SPA and API under one domain) is also valid: set
`ECOCHARGE_API_BASE_URL` to that public origin (or to the public API path
you proxy) and `ECOCHARGE_CLIENT_ORIGIN` to the same origin, then rebuild
the client.

## TLS and a reverse proxy

- Prefer HTTPS in production. Let's Encrypt (certbot) or your provider's
  certificate manager are typical choices.
- After enabling HTTPS, update `ECOCHARGE_CLIENT_ORIGIN` and
  `ECOCHARGE_API_BASE_URL` to the `https://` origins and rebuild the client.
- Do not put real certificate paths, hostnames, or account emails in this
  repository.

## Security checklist

- Store secrets in `.env` or a secrets manager, never in git
- Restrict the firewall to 80/443 (and SSH) when Nginx is in front; do not
  expose PostgreSQL publicly
- Replace any example passwords before the host is reachable from the
  internet
- Keep Docker images and the host OS updated
- The API container runs as a non-root user (`ecocharge`) and writes SQLite
  under `/data`

## Troubleshooting

| Symptom | What to check |
|---|---|
| Dashboard loads, API calls fail (CORS) | `ECOCHARGE_CLIENT_ORIGIN` must match the origin in the browser address bar (scheme + host + port) |
| Dashboard calls the wrong API host | Rebuild the client after changing `ECOCHARGE_API_BASE_URL` (build-time) |
| API exits on startup mentioning migrations / tables | Old `EnsureCreated` SQLite file or volume — remove it once, see [Database and migrations](#database-and-migrations) |
| `GET /health` fails | Container not up; port mapping; reverse proxy target |
| Empty station list | Unexpected after a fresh start — the API seeds 3 demo stations when the table is empty. Check API logs and `GET /api/stations` |
