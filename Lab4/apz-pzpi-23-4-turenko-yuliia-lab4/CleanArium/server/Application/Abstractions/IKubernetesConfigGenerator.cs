using Application.DTOs.Kubernetes;

namespace Application.Abstractions;

public interface IKubernetesConfigGenerator
{
    KubernetesConfigResponse Generate(KubernetesConfigRequest request);
}
