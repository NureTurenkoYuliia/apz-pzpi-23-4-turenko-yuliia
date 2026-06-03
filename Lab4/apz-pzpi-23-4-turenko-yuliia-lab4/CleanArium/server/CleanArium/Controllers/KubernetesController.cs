using Application.Abstractions;
using Application.DTOs.Kubernetes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArium.Controllers;

[ApiController]
[Route("api/kubernetes")]
[Authorize(Roles = "Admin")]
public class KubernetesController : ControllerBase
{
    private readonly IKubernetesService _service;
    private readonly IKubernetesConfigGenerator _generator;

    public KubernetesController(IKubernetesService service, IKubernetesConfigGenerator generator)
    {
        _service = service;
        _generator = generator;
    }

    [HttpGet("deployments")]
    public async Task<IActionResult> GetDeployments()
    {
        return Ok(await _service.GetDeploymentsAsync());
    }

    [HttpPost("scale")]
    public async Task<IActionResult> Scale(ScaleDeploymentRequest request)
    {
        await _service.ScaleDeploymentAsync(
            request.DeploymentName,
            request.Replicas);

        return NoContent();
    }

    [HttpPost("restart")]
    public async Task<IActionResult> Restart(RestartDeploymentRequest request)
    {
        await _service.RestartDeploymentAsync(request.DeploymentName);

        return NoContent();
    }

    [HttpGet("pods")]
    public async Task<IActionResult> GetPods()
    {
        return Ok(await _service.GetPodsAsync());
    }

    [HttpGet("logs/{deploymentName}")]
    public async Task<IActionResult> Logs(string deploymentName)
    {
        return Ok(await _service.GetLogsAsync(deploymentName));
    }

    [HttpPost("generate-config")]
    public IActionResult Generate([FromBody] KubernetesConfigRequest request)
    {
        var result = _generator.Generate(request);

        return Ok(result);
    }
}