import { StationCard } from '@/features/stations/StationCard';
import type { ChargingStation } from '@/types/station';

interface StationGridProps {
  stations: ChargingStation[];
  onStartCharge: (stationId: string, vehicleIdentifier: string) => Promise<void>;
  onStopCharge: (stationId: string) => Promise<void>;
}

export function StationGrid({ stations, onStartCharge, onStopCharge }: StationGridProps) {
  if (stations.length === 0) {
    return (
      <div className="rounded-2xl border border-dashed border-graphite-200 bg-white p-10 text-center text-graphite-500">
        No charging stations yet. Create one from the API to see it appear here.
      </div>
    );
  }

  return (
    <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
      {stations.map((station) => (
        <StationCard
          key={station.id}
          station={station}
          onStartCharge={onStartCharge}
          onStopCharge={onStopCharge}
        />
      ))}
    </div>
  );
}
