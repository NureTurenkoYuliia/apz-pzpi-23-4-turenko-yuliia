using Application.Abstractions;
using Application.DTOs.Kubernetes;
using k8s;
using k8s.Models;

namespace Infrastructure.Services;

public class KubernetesService : IKubernetesService
{
    private readonly IKubernetes _client;
    private const string Namespace = "default";

    public KubernetesService()
    {
        var config = KubernetesClientConfiguration.InClusterConfig();

        _client = new Kubernetes(config);
    }

    public async Task<List<DeploymentDto>> GetDeploymentsAsync()
    {
        var deployments = await _client.AppsV1.ListNamespacedDeploymentAsync(Namespace);

        return deployments.Items.Select(d => new DeploymentDto
        {
            Name = d.Metadata.Name!,
            Replicas = d.Spec.Replicas ?? 0,
            ReadyReplicas = d.Status.ReadyReplicas ?? 0,
            AvailableReplicas = d.Status.AvailableReplicas ?? 0
        }).ToList();
    }

    public async Task ScaleDeploymentAsync(string deploymentName, int replicas)
    {
        var deployment = await _client.AppsV1.ReadNamespacedDeploymentAsync(deploymentName, Namespace);

        deployment.Spec.Replicas = replicas;

        await _client.AppsV1.ReplaceNamespacedDeploymentAsync(
            deployment,
            deploymentName,
            Namespace);
    }

    public async Task RestartDeploymentAsync(string deploymentName)
    {
        var deployment = await _client.AppsV1.ReadNamespacedDeploymentAsync(deploymentName, Namespace);

        deployment.Spec.Template.Metadata ??= new V1ObjectMeta();

        deployment.Spec.Template.Metadata.Annotations ??= new Dictionary<string, string>();

        deployment.Spec.Template.Metadata.Annotations[
            "kubectl.kubernetes.io/restartedAt"] =
            DateTime.UtcNow.ToString("O");

        await _client.AppsV1.ReplaceNamespacedDeploymentAsync(
            deployment,
            deploymentName,
            Namespace);
    }

    public async Task<List<PodDto>> GetPodsAsync()
    {
        var pods = await _client.CoreV1.ListNamespacedPodAsync("default");

        return pods.Items.Select(p => new PodDto
        {
            Name = p.Metadata.Name,
            Status = p.Status.Phase,
            Node = p.Spec.NodeName,
            PodIp = p.Status.PodIP
        }).ToList();
    }

    public async Task<PodLogsDto> GetLogsAsync(string deploymentName)
    {
        var deployment = await _client.AppsV1.ReadNamespacedDeploymentAsync(deploymentName, Namespace);
        var labels = deployment.Spec.Selector.MatchLabels;
        var selector = string.Join(",", labels.Select(x => $"{x.Key}={x.Value}"));
        var pods = await _client.CoreV1.ListNamespacedPodAsync(Namespace, labelSelector: selector);
        var pod = pods.Items.First();
        var logs = await _client.CoreV1.ReadNamespacedPodLogAsync(
            pod.Metadata.Name,
            Namespace,
            tailLines: 100);

        string logsText = string.Empty;
        using (var reader = new StreamReader(logs))
        {
            logsText = await reader.ReadToEndAsync();
        }

        return new PodLogsDto
        {
            PodName = pod.Metadata.Name!,
            Logs = logsText
        };
    }
}