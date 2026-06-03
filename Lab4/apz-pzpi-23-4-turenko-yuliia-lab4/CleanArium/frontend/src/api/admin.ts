import apiClient from './client';
import {
  CommandAlarmCorrelationDto,
  ModeratorDto,
  SystemSettingsDto,
  UpdateSystemSettingsRequest,
  UserActivityDailyDto,
} from '../types';

export const adminApi = {
  makeModerator: async (userId: number): Promise<void> => {
    await apiClient.post(`/api/admin/${userId}/make-moderator`);
  },

  removeModerator: async (userId: number): Promise<void> => {
    await apiClient.post(`/api/admin/${userId}/remove-moderator`);
  },

  getModerators: async (): Promise<ModeratorDto[]> => {
    const res = await apiClient.get<ModeratorDto[]>('/api/admin/moderators');
    return res.data;
  },

  getSystemSettings: async (): Promise<SystemSettingsDto> => {
    const res = await apiClient.get<SystemSettingsDto>('/api/admin/system-settings');
    return res.data;
  },

  updateSystemSettings: async (data: UpdateSystemSettingsRequest): Promise<void> => {
    await apiClient.put('/api/admin/update-system-settings', data);
  },

  getDailyActivity: async (days: number): Promise<UserActivityDailyDto[]> => {
    const res = await apiClient.get<UserActivityDailyDto[]>(`/api/admin/daily-activity/${days}`);
    return res.data;
  },

  getCorrelation: async (from: Date, to: Date): Promise<CommandAlarmCorrelationDto[]> => {
    const res = await apiClient.get<CommandAlarmCorrelationDto[]>(
      '/api/admin/command-alarm-correlation',
      { params: { from: from.toISOString(), to: to.toISOString() } }
    );
    return res.data;
  },
};
