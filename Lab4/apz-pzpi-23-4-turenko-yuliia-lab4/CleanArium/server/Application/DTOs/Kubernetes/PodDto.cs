namespace Application.DTOs.Kubernetes;

public class PodDto
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Node { get; set; } = string.Empty;
    public string PodIp { get; set; } = string.Empty;
}
