namespace Application.DTOs.Kubernetes;

public class RestartDeploymentRequest
{
    public string DeploymentName { get; set; } = string.Empty;
}
