import apiClient from './client';
import {
  CreateScheduledCommandRequest,
  ScheduledCommandDto,
  UpdateScheduledCommandRequest,
} from '../types';

export const scheduledCommandApi = {
  getByDevice: async (deviceId: number): Promise<ScheduledCommandDto[]> => {
    const res = await apiClient.get<ScheduledCommandDto[]>(
      `/api/scheduledcommand/get-all-by-device/${deviceId}`
    );
    return res.data;
  },

  create: async (data: CreateScheduledCommandRequest): Promise<number> => {
    const res = await apiClient.post<number>('/api/scheduledcommand/create', data);
    return res.data;
  },

  update: async (data: UpdateScheduledCommandRequest): Promise<void> => {
    await apiClient.put('/api/scheduledcommand/update', data);
  },

  delete: async (id: number): Promise<void> => {
    await apiClient.delete(`/api/scheduledcommand/delete/${id}`);
  },

  deactivate: async (id: number): Promise<void> => {
    await apiClient.post(`/api/scheduledcommand/${id}/deactivate`);
  },

  import: async (deviceId: number, file: File): Promise<void> => {
    const form = new FormData();
    form.append('file', file);
    await apiClient.post(`/api/scheduledcommand/${deviceId}/import`, form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },
};