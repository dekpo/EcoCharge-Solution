import { Card } from '@/components/ui/Card';

interface KpiBannerProps {
  totalStations: number;
  totalPowerKw: number;
  activeSessionsCount: number;
}

interface KpiTileProps {
  label: string;
  value: string;
  accent?: string;
}

function KpiTile({ label, value, accent = 'text-graphite-900' }: KpiTileProps) {
  return (
    <Card className="flex flex-1 flex-col gap-1">
      <span className="text-sm font-medium text-graphite-500">{label}</span>
      <span className={`text-3xl font-bold tracking-tight ${accent}`}>{value}</span>
    </Card>
  );
}

export function KpiBanner({ totalStations, totalPowerKw, activeSessionsCount }: KpiBannerProps) {
  return (
    <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
      <KpiTile label="Charging stations" value={totalStations.toString()} />
      <KpiTile
        label="Live power draw"
        value={`${totalPowerKw.toFixed(1)} kW`}
        accent="text-brand-700"
      />
      <KpiTile label="Active sessions" value={activeSessionsCount.toString()} accent="text-amber-600" />
    </div>
  );
}
