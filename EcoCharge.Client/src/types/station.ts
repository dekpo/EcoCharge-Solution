/**
 * Mirrors EcoCharge.Domain.Enums.StationStatus on the backend.
 * Keep in sync manually until an OpenAPI-generated client is introduced.
 */
export const StationStatus = {
  Available: 'Available',
  Charging: 'Charging',
  Maintenance: 'Maintenance',
} as const;

export type StationStatus = (typeof StationStatus)[keyof typeof StationStatus];

/** Accepts the API string names or the numeric enum values System.Text.Json uses by default. */
export function asStationStatus(value: unknown): StationStatus {
  if (value === StationStatus.Charging || value === 1 || value === '1') {
    return StationStatus.Charging;
  }

  if (value === StationStatus.Maintenance || value === 2 || value === '2') {
    return StationStatus.Maintenance;
  }

  return StationStatus.Available;
}

export interface ChargingStation {
  id: string;
  name: string;
  location: string;
  maxPowerKw: number;
  status: StationStatus;
}

export interface ChargeSession {
  id: string;
  chargingStationId: string;
  vehicleIdentifier: string;
  startedAtUtc: string;
  stoppedAtUtc: string | null;
  currentPowerKw: number;
  totalEnergyKwh: number;
  isActive: boolean;
}

export interface CreateChargingStationRequest {
  name: string;
  location: string;
  maxPowerKw: number;
}

/** A single line pushed by the backend BackgroundService simulation, surfaced to the UI as a "live log". */
export interface ConsumptionLogEntry {
  id: string;
  chargingStationId: string;
  stationName: string;
  currentPowerKw: number;
  totalEnergyKwh: number;
  recordedAtUtc: string;
}
