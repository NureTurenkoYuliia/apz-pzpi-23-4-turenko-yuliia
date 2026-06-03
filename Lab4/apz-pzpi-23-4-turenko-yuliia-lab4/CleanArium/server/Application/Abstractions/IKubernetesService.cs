using Application.DTOs.Kubernetes;

namespace Application.Abstractions;

public interface IKubernetesService
{
    Task<List<DeploymentDto>> GetDeploymentsAsync();
    Task ScaleDeploymentAsync(string deploymentName, int replicas);
    Task RestartDeploymentAsync(string deploymentName);
    Task<List<PodDto>> GetPodsAsync();
    Task<PodLogsDto> GetLogsAsync(string deploymentName);
}
