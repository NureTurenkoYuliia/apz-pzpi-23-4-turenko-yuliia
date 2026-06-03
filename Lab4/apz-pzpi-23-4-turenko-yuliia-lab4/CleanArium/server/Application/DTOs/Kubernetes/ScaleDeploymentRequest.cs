namespace Application.DTOs.Kubernetes;

public class ScaleDeploymentRequest
{
    public string DeploymentName { get; set; } = string.Empty;

    public int Replicas { get; set; }
}