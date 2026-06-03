import apiClient from './client';
import { NotificationDto } from '../types';

export const notificationApi = {
  getAll: async (): Promise<NotificationDto[]> => {
    const res = await apiClient.get<NotificationDto[]>('/api/notification/get-all');
    return res.data;
  },

  getUnreadCount: async (): Promise<number> => {
    const res = await apiClient.get<number>('/api/notification/unread-count');
    return res.data;
  },

  markAsRead: async (id: number): Promise<void> => {
    await apiClient.post(`/api/notification/read/${id}`);
  },
};