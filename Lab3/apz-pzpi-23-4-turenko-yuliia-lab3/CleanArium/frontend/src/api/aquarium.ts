import apiClient from './client';
import { AquariumDto, CreateAquariumRequest, UpdateAquariumRequest } from '../types';

export const aquariumApi = {
  getAll: async (): Promise<AquariumDto[]> => {
    const res = await apiClient.get<AquariumDto[]>('/api/aquarium/get-all-by-user');
    return res.data;
  },

  create: async (data: CreateAquariumRequest): Promise<number> => {
    const res = await apiClient.post<number>('/api/aquarium/create', data);
    return res.data;
  },

  update: async (data: UpdateAquariumRequest): Promise<void> => {
    await apiClient.put('/api/aquarium/update', data);
  },

  delete: async (id: number): Promise<void> => {
    await apiClient.delete('/api/aquarium/delete', { data: id });
  },

  exportCsv: async (): Promise<Blob> => {
    const res = await apiClient.get('/api/aquarium/export-csv', { responseType: 'blob' });
    return res.data;
  },

  exportJson: async (): Promise<Blob> => {
    const res = await apiClient.get('/api/aquarium/export/json', { responseType: 'blob' });
    return res.data;
  },

  exportPdf: async (): Promise<Blob> => {
    const res = await apiClient.get('/api/aquarium/export/pdf', { responseType: 'blob' });
    return res.data;
  },

  import: async (file: File): Promise<void> => {
    const form = new FormData();
    form.append('file', file);
    await apiClient.post('/api/aquarium/import', form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },
};