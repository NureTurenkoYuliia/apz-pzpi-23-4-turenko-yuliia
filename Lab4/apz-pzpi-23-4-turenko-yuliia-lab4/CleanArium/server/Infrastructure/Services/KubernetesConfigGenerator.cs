using Application.Abstractions;
using Application.DTOs.Kubernetes;

namespace Infrastructure.Services;

public class KubernetesConfigGenerator : IKubernetesConfigGenerator
{
    public KubernetesConfigResponse Generate(KubernetesConfigRequest request)
    {
        var deployment = GenerateDeployment(request);
        var service = GenerateService(request);
        var hpa = GenerateHpa(request);

        return new KubernetesConfigResponse
        {
            DeploymentYaml = deployment,
            ServiceYaml = service,
            HpaYaml = hpa
        };
    }

    private string GenerateDeployment(KubernetesConfigRequest r)
    {
        return $"""
apiVersion: apps/v1
kind: Deployment
metadata:
  name: {r.AppName}
spec:
  replicas: {r.Replicas}
  selector:
    matchLabels:
      app: {r.AppName}
  template:
    metadata:
      labels:
        app: {r.AppName}
    spec:
      containers:
      - name: {r.AppName}
        image: {r.Image}
        ports:
        - containerPort: {r.ContainerPort}
        resources:
          requests:
            cpu: {r.CpuRequest}
            memory: {r.MemoryRequest}
""";
    }

    private string GenerateService(KubernetesConfigRequest r)
    {
        return $"""
apiVersion: v1
kind: Service
metadata:
  name: {r.AppName}-service
spec:
  selector:
    app: {r.AppName}
  ports:
  - protocol: TCP
    port: 80
    targetPort: {r.ContainerPort}
  type: ClusterIP
""";
    }

    private string GenerateHpa(
        KubernetesConfigRequest r)
    {
        return $"""
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: {r.AppName}-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: {r.AppName}
  minReplicas: {r.MinReplicas}
  maxReplicas: {r.MaxReplicas}
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
""";
    }
}