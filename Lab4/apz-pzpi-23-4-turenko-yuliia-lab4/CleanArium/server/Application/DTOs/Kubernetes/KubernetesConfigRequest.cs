namespace Application.DTOs.Kubernetes;

public class KubernetesConfigRequest
{
    public string AppName { get; set; } = null!;

    public string Image { get; set; } = null!;

    public int Replicas { get; set; }

    public int ContainerPort { get; set; }

    public string CpuRequest { get; set; } = "100m";

    public string MemoryRequest { get; set; } = "128Mi";

    public int MinReplicas { get; set; } = 1;

    public int MaxReplicas { get; set; } = 5;
}
