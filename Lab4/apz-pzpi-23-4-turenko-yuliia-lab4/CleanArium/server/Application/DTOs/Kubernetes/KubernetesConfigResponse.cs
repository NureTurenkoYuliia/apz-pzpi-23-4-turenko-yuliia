namespace Application.DTOs.Kubernetes;

public class KubernetesConfigResponse
{
    public string DeploymentYaml { get; set; } = null!;

    public string ServiceYaml { get; set; } = null!;

    public string HpaYaml { get; set; } = null!;
}
