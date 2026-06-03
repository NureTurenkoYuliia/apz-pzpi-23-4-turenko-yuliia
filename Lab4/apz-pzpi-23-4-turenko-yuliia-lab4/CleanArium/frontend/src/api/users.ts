import apiClient from './client';
import { InactiveUserDto, PreviewUserDto, UserDto } from '../types';

export const userApi = {
  getAllUsers: async (): Promise<PreviewUserDto[]> => {
    const res = await apiClient.get<PreviewUserDto[]>('/api/user/all-users');
    return res.data;
  },

  getUserById: async (id: number): Promise<UserDto> => {
    const res = await apiClient.get<UserDto>(`/api/user/${id}/user`);
    return res.data;
  },

  getInactiveUsers: async (inactiveDays: number): Promise<InactiveUserDto[]> => {
    const res = await apiClient.get<InactiveUserDto[]>(`/api/user/inactive-users/${inactiveDays}`);
    return res.data;
  },

  blockUser: async (id: number): Promise<void> => {
    await apiClient.post(`/api/user/block/${id}`);
  },

  unblockUser: async (id: number): Promise<void> => {
    await apiClient.post(`/api/user/ublock/${id}`);
  },

  deleteUser: async (id: number): Promise<void> => {
    await apiClient.delete(`/api/user/delete/${id}`);
  },
};
