import apiClient from './client';
import {
  DeploymentDto,
  KubernetesConfigRequest,
  KubernetesConfigResponse,
  PodDto,
  PodLogsDto,
  RestartDeploymentRequest,
  ScaleDeploymentRequest,
} from '../types';

export const kubernetesApi = {
  getDeployments: async (): Promise<DeploymentDto[]> => {
    const res = await apiClient.get<DeploymentDto[]>('/api/kubernetes/deployments');
    return res.data;
  },

  getPods: async (): Promise<PodDto[]> => {
    const res = await apiClient.get<PodDto[]>('/api/kubernetes/pods');
    return res.data;
  },

  scale: async (data: ScaleDeploymentRequest): Promise<void> => {
    await apiClient.post('/api/kubernetes/scale', data);
  },

  restart: async (data: RestartDeploymentRequest): Promise<void> => {
    await apiClient.post('/api/kubernetes/restart', data);
  },

  getLogs: async (deploymentName: string): Promise<PodLogsDto> => {
    const res = await apiClient.get<PodLogsDto>(`/api/kubernetes/logs/${deploymentName}`);
    return res.data;
  },

  generateConfig: async (data: KubernetesConfigRequest): Promise<KubernetesConfigResponse> => {
    const res = await apiClient.post<KubernetesConfigResponse>('/api/kubernetes/generate-config', data);
    return res.data;
  },
};