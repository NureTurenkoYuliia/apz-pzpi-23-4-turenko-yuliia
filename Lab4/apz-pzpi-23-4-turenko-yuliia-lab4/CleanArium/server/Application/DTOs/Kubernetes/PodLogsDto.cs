namespace Application.DTOs.Kubernetes;

public class PodLogsDto
{
    public string PodName { get; set; } = string.Empty;

    public string Logs { get; set; } = string.Empty;
}