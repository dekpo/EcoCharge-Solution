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
