import apiClient from './client';
import { AuthResponseDto, LoginRequest, LogoutRequest, RefreshTokenRequest, RegisterRequest } from '../types';

export const authApi = {
  login: async (data: LoginRequest): Promise<AuthResponseDto> => {
    const res = await apiClient.post<AuthResponseDto>('/api/auth/login', data);
    return res.data;
  },

  register: async (data: RegisterRequest): Promise<AuthResponseDto> => {
    const res = await apiClient.post<AuthResponseDto>('/api/auth/register', data);
    return res.data;
  },

  logout: async (data: LogoutRequest): Promise<void> => {
    await apiClient.post('/api/auth/logout', data);
  },

  refreshToken: async (data: RefreshTokenRequest): Promise<AuthResponseDto> => {
    const res = await apiClient.post<AuthResponseDto>('/api/auth/refresh-token', data);
    return res.data;
  },
};
