import { Card } from '@/components/ui/Card';
import type { ConsumptionLogEntry } from '@/types/station';

interface ConsumptionLogProps {
  entries: ConsumptionLogEntry[];
}

function formatTime(isoTimestamp: string): string {
  return new Date(isoTimestamp).toLocaleTimeString('en-CH', { hour12: false });
}

export function ConsumptionLog({ entries }: ConsumptionLogProps) {
  return (
    <Card className="flex flex-col gap-3">
      <div className="flex items-center justify-between">
        <h3 className="font-semibold text-graphite-900">Live consumption log</h3>
        <span className="flex items-center gap-1.5 text-xs font-medium text-brand-600">
          <span className="h-1.5 w-1.5 animate-pulse rounded-full bg-brand-500" />
          streaming from backend worker
        </span>
      </div>

      <div className="max-h-72 overflow-y-auto rounded-lg bg-graphite-900 p-3 font-mono text-xs text-graphite-100">
        {entries.length === 0 ? (
          <p className="text-graphite-400">Waiting for active charge sessions…</p>
        ) : (
          <ul className="space-y-1">
            {entries.map((entry) => (
              <li key={entry.id} className="whitespace-nowrap">
                <span className="text-graphite-500">[{formatTime(entry.recordedAtUtc)}]</span>{' '}
                <span className="text-brand-400">{entry.stationName}</span>{' '}
                <span>
                  draw={entry.currentPowerKw.toFixed(2)}kW total={entry.totalEnergyKwh.toFixed(3)}kWh
                </span>
              </li>
            ))}
          </ul>
        )}
      </div>
    </Card>
  );
}
