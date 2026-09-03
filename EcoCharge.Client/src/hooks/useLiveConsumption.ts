import { useEffect, useRef, useState } from 'react';
import { POLL_INTERVAL_MS } from '@/services/apiClient';
import { stationsService } from '@/services/stationsService';
import type { ConsumptionLogEntry } from '@/types/station';

const MAX_ENTRIES = 25;

interface UseLiveConsumptionResult {
  entries: ConsumptionLogEntry[];
  totalPowerKw: number;
  activeSessionsCount: number;
}

/**
 * Polls the consumption log produced by the backend's BackgroundService
 * (which simulates kW draw every 5s for active charge sessions) and keeps
 * a rolling window of the most recent entries for the "live log" UI.
 */
export function useLiveConsumption(): UseLiveConsumptionResult {
  const [entries, setEntries] = useState<ConsumptionLogEntry[]>([]);
  const seenIds = useRef<Set<string>>(new Set());

  useEffect(() => {
    let cancelled = false;

    const poll = async () => {
      try {
        const latest = await stationsService.getConsumptionLog();
        if (cancelled) return;

        const fresh = latest.filter((entry) => !seenIds.current.has(entry.id));
        fresh.forEach((entry) => seenIds.current.add(entry.id));

        if (fresh.length > 0) {
          setEntries((prev) => [...fresh, ...prev].slice(0, MAX_ENTRIES));
        }
      } catch {
        // Silently ignore transient polling errors; useStations already
        // surfaces the "API unreachable" state to the user.
      }
    };

    void poll();
    const interval = setInterval(() => void poll(), POLL_INTERVAL_MS);
    return () => {
      cancelled = true;
      clearInterval(interval);
    };
  }, []);

  // Keep only the most recent reading per station to avoid double-counting
  // power across several historical log entries for the same session.
  const latestReadingByStation = new Map<string, number>();
  for (const entry of entries) {
    if (!latestReadingByStation.has(entry.chargingStationId)) {
      latestReadingByStation.set(entry.chargingStationId, entry.currentPowerKw);
    }
  }

  const totalPowerKw = Array.from(latestReadingByStation.values()).reduce((sum, kw) => sum + kw, 0);
  const activeSessionsCount = latestReadingByStation.size;

  return { entries, totalPowerKw, activeSessionsCount };
}
