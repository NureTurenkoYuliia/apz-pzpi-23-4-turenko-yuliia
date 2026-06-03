import apiClient from './client';
import {
  CreateDeviceRequest,
  DeviceDto,
  ExecuteCommandRequest,
  SensorDataDto,
  UpdateDeviceRequest,
} from '../types';

export const deviceApi = {
  getByAquarium: async (aquariumId: number): Promise<DeviceDto[]> => {
    const res = await apiClient.get<DeviceDto[]>(
      `/api/device/get-devices-by-aquarium/${aquariumId}`
    );
    return res.data;
  },

  create: async (data: CreateDeviceRequest): Promise<number> => {
    const res = await apiClient.post<number>('/api/device/create', data);
    return res.data;
  },

  update: async (data: UpdateDeviceRequest): Promise<void> => {
    await apiClient.put('/api/device/update', data);
  },

  delete: async (deviceId: number): Promise<void> => {
    await apiClient.delete(`/api/device/delete/${deviceId}`);
  },

  getSensorData: async (deviceId: number): Promise<SensorDataDto> => {
    const res = await apiClient.get<SensorDataDto>(
      `/api/device/get-sensor-data/${deviceId}`
    );
    return res.data;
  },

  executeCommand: async (deviceId: number, data: ExecuteCommandRequest): Promise<void> => {
    await apiClient.post(`/api/device/${deviceId}/executed-command`, data);
  },
};