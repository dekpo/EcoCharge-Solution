# Screenshots

Public product images for the root `README.md`. This folder **is tracked by
Git**. Do not put screenshots under `docs/` — that path is gitignored
(internal notes only).

| File | What it shows |
|---|---|
| `01-dashboard-kpis.png` | Header, Switzerland demo badge, KPI tiles (stations / live kW / sessions) |
| `02-stations-start.png` | Three seeded stations with **Start charge** |
| `03-start-and-stop.png` | Mixed states: Available + Start, Charging + Stop, consumption log |
| `04-all-charging.png` | All three stations **Charging** / **Stop charge** |
| `05-consumption-log.png` | Live log streamed from the 5-second backend worker |
| `06-docker-desktop.png` | Docker Desktop: `ecocharge` stack running (portable, no VPS) |

Not included on purpose: the empty dashboard (pre-seed) and the blank
`http://localhost:5080` page (the API has no HTML route `/`; use `/health`
or the client on `:5173`).
