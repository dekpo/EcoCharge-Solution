import { useCallback, useEffect, useState } from 'react';
import { POLL_INTERVAL_MS } from '@/services/apiClient';
import { stationsService } from '@/services/stationsService';
import type { ChargingStation } from '@/types/station';

interface UseStationsResult {
  stations: ChargingStation[];
  isLoading: boolean;
  error: string | null;
  startCharge: (stationId: string, vehicleIdentifier: string) => Promise<void>;
  stopCharge: (stationId: string) => Promise<void>;
  refetch: () => Promise<void>;
}

/**
 * Isolates all "charging stations list" data-fetching/mutation logic away
 * from presentational components. Polls the backend so status changes
 * produced by the BackgroundService worker are reflected in near-real-time.
 */
export function useStations(): UseStationsResult {
  const [stations, setStations] = useState<ChargingStation[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const refetch = useCallback(async () => {
    try {
      const data = await stationsService.getStations();
      setStations(data);
      setError(null);
    } catch {
      setError('Unable to reach EcoCharge API. Is the backend running?');
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    void refetch();
    const interval = setInterval(() => void refetch(), POLL_INTERVAL_MS);
    return () => clearInterval(interval);
  }, [refetch]);

  const startCharge = useCallback(
    async (stationId: string, vehicleIdentifier: string) => {
      await stationsService.startCharge(stationId, vehicleIdentifier);
      await refetch();
    },
    [refetch],
  );

  const stopCharge = useCallback(
    async (stationId: string) => {
      await stationsService.stopCharge(stationId);
      await refetch();
    },
    [refetch],
  );

  return { stations, isLoading, error, startCharge, stopCharge, refetch };
}
