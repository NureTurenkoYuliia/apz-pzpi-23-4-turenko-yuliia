namespace Application.DTOs.Kubernetes;

public class DeploymentDto
{
    public string Name { get; set; } = string.Empty;

    public int Replicas { get; set; }

    public int ReadyReplicas { get; set; }

    public int AvailableReplicas { get; set; }
}