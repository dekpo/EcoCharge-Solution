import { ConsumptionLog } from '@/features/stations/ConsumptionLog';
import { KpiBanner } from '@/features/stations/KpiBanner';
import { StationGrid } from '@/features/stations/StationGrid';
import { useLiveConsumption } from '@/hooks/useLiveConsumption';
import { useStations } from '@/hooks/useStations';

export function Dashboard() {
  const { stations, isLoading, error, startCharge, stopCharge } = useStations();
  const { entries, totalPowerKw, activeSessionsCount } = useLiveConsumption();

  return (
    <div className="min-h-screen bg-graphite-50">
      <header className="border-b border-graphite-100 bg-white">
        <div className="mx-auto flex max-w-6xl items-center justify-between px-6 py-5">
          <div className="flex items-center gap-2.5">
            <span className="flex h-8 w-8 items-center justify-center rounded-lg bg-brand-600 text-sm font-bold text-white">
              EC
            </span>
            <div>
              <h1 className="text-lg font-bold leading-tight text-graphite-900">EcoCharge</h1>
              <p className="text-xs text-graphite-500">EV fleet charging management</p>
            </div>
          </div>
          <span className="rounded-full bg-graphite-100 px-3 py-1 text-xs font-medium text-graphite-600">
            Switzerland · Demo environment
          </span>
        </div>
      </header>

      <main className="mx-auto max-w-6xl space-y-6 px-6 py-8">
        {error && (
          <div className="rounded-xl border border-rose-200 bg-rose-50 px-4 py-3 text-sm text-rose-700">
            {error}
          </div>
        )}

        <KpiBanner
          totalStations={stations.length}
          totalPowerKw={totalPowerKw}
          activeSessionsCount={activeSessionsCount}
        />

        <section>
          <h2 className="mb-3 text-base font-semibold text-graphite-900">Charging stations</h2>
          {isLoading ? (
            <p className="text-sm text-graphite-500">Loading stations…</p>
          ) : (
            <StationGrid stations={stations} onStartCharge={startCharge} onStopCharge={stopCharge} />
          )}
        </section>

        <section>
          <ConsumptionLog entries={entries} />
        </section>
      </main>
    </div>
  );
}
