import { useState } from 'react';
import { StatusBadge } from '@/components/ui/Badge';
import { Button } from '@/components/ui/Button';
import { Card } from '@/components/ui/Card';
import type { ChargingStation } from '@/types/station';

interface StationCardProps {
  station: ChargingStation;
  onStartCharge: (stationId: string, vehicleIdentifier: string) => Promise<void>;
  onStopCharge: (stationId: string) => Promise<void>;
}

export function StationCard({ station, onStartCharge, onStopCharge }: StationCardProps) {
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleStart = async () => {
    setIsSubmitting(true);
    try {
      await onStartCharge(station.id, `DEMO-${station.id.slice(0, 6).toUpperCase()}`);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleStop = async () => {
    setIsSubmitting(true);
    try {
      await onStopCharge(station.id);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Card className="flex flex-col gap-4">
      <div className="flex items-start justify-between">
        <div>
          <h3 className="font-semibold text-graphite-900">{station.name}</h3>
          <p className="text-sm text-graphite-500">{station.location}</p>
        </div>
        <StatusBadge status={station.status} />
      </div>

      <p className="text-sm text-graphite-600">
        Max power: <span className="font-medium text-graphite-900">{station.maxPowerKw} kW</span>
      </p>

      <div className="mt-auto flex gap-2">
        {station.status !== 'Charging' ? (
          <Button
            variant="primary"
            className="flex-1"
            disabled={isSubmitting || station.status === 'Maintenance'}
            onClick={handleStart}
          >
            Start charge
          </Button>
        ) : (
          <Button variant="danger" className="flex-1" disabled={isSubmitting} onClick={handleStop}>
            Stop charge
          </Button>
        )}
      </div>
    </Card>
  );
}
