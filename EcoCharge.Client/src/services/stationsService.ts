import { apiClient } from '@/services/apiClient';
import { asStationStatus, type ChargeSession, type ChargingStation, type ConsumptionLogEntry, type CreateChargingStationRequest } from '@/types/station';

/**
 * Typed wrapper around the EcoCharge.Api charging-stations endpoints.
 * Every backend integration point for the "stations" feature lives here,
 * so components/hooks never call axios/fetch directly.
 */
export const stationsService = {
  async getStations(): Promise<ChargingStation[]> {
    const { data } = await apiClient.get<ChargingStation[]>('/api/stations');
    return data.map((station) => ({ ...station, status: asStationStatus(station.status) }));
  },

  async createStation(payload: CreateChargingStationRequest): Promise<ChargingStation> {
    const { data } = await apiClient.post<ChargingStation>('/api/stations', payload);
    return { ...data, status: asStationStatus(data.status) };
  },

  async startCharge(stationId: string, vehicleIdentifier: string): Promise<ChargeSession> {
    const { data } = await apiClient.post<ChargeSession>(`/api/stations/${stationId}/start`, {
      vehicleIdentifier,
    });
    return data;
  },

  async stopCharge(stationId: string): Promise<ChargeSession> {
    const { data } = await apiClient.post<ChargeSession>(`/api/stations/${stationId}/stop`);
    return data;
  },

  async getActiveSessions(): Promise<ChargeSession[]> {
    const { data } = await apiClient.get<ChargeSession[]>('/api/sessions/active');
    return data;
  },

  async getConsumptionLog(): Promise<ConsumptionLogEntry[]> {
    const { data } = await apiClient.get<ConsumptionLogEntry[]>('/api/sessions/consumption-log');
    return data;
  },
};
