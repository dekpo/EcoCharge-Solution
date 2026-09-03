# Contributing to EcoCharge

This document describes the workflow and conventions used on this
repository.

## Branching strategy

We use a lightweight trunk-based flow:

- `main` — always deployable. Protected in spirit (even solo, avoid force-push).
- Short-lived feature branches off `main`, named `<type>/<short-kebab-description>`:

| Type | Use for |
|---|---|
| `feat/` | New feature or endpoint |
| `fix/` | Bug fix |
| `chore/` | Tooling, deps, config, maintenance |
| `docs/` | Documentation only |
| `refactor/` | Code change with no behavior change |
| `test/` | Adding/adjusting tests only |
| `infra/` | Docker, CI, deployment scripts |

Examples: `feat/charging-station-crud`, `feat/dashboard-kpis`,
`infra/docker-compose-local`, `docs/deployment-guide`.

Merge feature branches back into `main` via pull request (even solo — it
keeps a clean, reviewable history and mirrors real-world team workflow,
which is useful to show to recruiters).

## Commit messages — Conventional Commits

Format: `<type>(<scope>): <imperative summary>`

Types: `feat`, `fix`, `chore`, `docs`, `refactor`, `test`, `infra`, `style`, `perf`.
Scopes: `domain`, `application`, `infrastructure`, `api`, `client`, `tests`, `docs`, `infra`, `deploy`.

Examples:
```
feat(domain): add ChargingStation entity and StationStatus enum
feat(application): implement CreateChargingStationCommand with validation
feat(infrastructure): add EF Core SQLite configuration and migrations
feat(api): expose charging station CRUD endpoints with ProblemDetails errors
feat(client): add KPI banner and station grid to dashboard
infra(docker): add docker-compose for local API + Postgres profile
docs(readme): document local setup and deployment steps
test(application): cover CreateChargingStationCommandHandler with xUnit/Moq
```

## Code style

- **C#**: file-scoped namespaces, nullable reference types enabled, primary
  constructors where they improve readability, expression-bodied members for
  simple cases, async all the way down for I/O.
- **TypeScript/React**: strict mode, functional components, custom hooks to
  isolate logic from presentation, no `any`.
- Run `dotnet format` and the frontend linter before proposing a commit.

## Tests

- Backend: xUnit + Moq, at minimum one test per non-trivial Application
  handler / Domain rule.
- Run `dotnet test` before proposing any backend commit.

## Language policy

English for everything in this repo (code, docs, commits, branches, UI).

## Shell on Windows

On Windows, documented commands assume **classic `cmd.exe`**, not PowerShell.
Use `scripts\dotnet.cmd` for any backend `dotnet` invocation (the `.ps1`
wrapper is optional). In cmd, chain commands with `&` (sequential) or
`&&` (stop on first failure).
