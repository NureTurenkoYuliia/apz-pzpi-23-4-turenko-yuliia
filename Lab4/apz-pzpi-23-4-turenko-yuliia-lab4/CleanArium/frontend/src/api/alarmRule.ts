import apiClient from './client';
import {
  AlarmRuleAnalysisDto,
  AlarmRuleDto,
  CreateAlarmRuleRequest,
  UpdateAlarmRuleRequest,
} from '../types';

export const alarmRuleApi = {
  getByDevice: async (deviceId: number): Promise<AlarmRuleDto[]> => {
    const res = await apiClient.get<AlarmRuleDto[]>(
      `/api/alarmrule/get-all-by-device/${deviceId}`
    );
    return res.data;
  },

  create: async (data: CreateAlarmRuleRequest): Promise<number> => {
    const res = await apiClient.post<number>('/api/alarmrule/create', data);
    return res.data;
  },

  update: async (data: UpdateAlarmRuleRequest): Promise<void> => {
    await apiClient.put('/api/alarmrule/update', data);
  },

  delete: async (id: number): Promise<void> => {
    await apiClient.delete('/api/alarmrule/delete', { data: id });
  },

  deactivate: async (id: number): Promise<void> => {
    await apiClient.post(`/api/alarmrule/${id}/deactivate`);
  },

  analyze: async (
    id: number,
    from: Date,
    to: Date
  ): Promise<AlarmRuleAnalysisDto> => {
    const res = await apiClient.get<AlarmRuleAnalysisDto>(
      `/api/alarmrule/${id}/analysis`,
      { params: { from: from.toISOString(), to: to.toISOString() } }
    );
    return res.data;
  },

  import: async (deviceId: number, file: File): Promise<void> => {
    const form = new FormData();
    form.append('file', file);
    await apiClient.post(`/api/alarmrule/${deviceId}/import`, form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },
};