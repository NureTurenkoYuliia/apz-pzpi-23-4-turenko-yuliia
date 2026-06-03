using Application.Abstractions;
using Application.SystemSettings.Commands.Update;
using Application.SystemSettings.Queries.GetAll;
using Application.Users.Commands.AssignModerator;
using Application.Users.Commands.RemoveModerator;
using Application.Users.Queries.GetModerators;
using CleanArium.Contracts.SystemSettings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace CleanArium.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{

    private readonly IMediator _mediator;
    private readonly IUserActivityService _activityService;
    private readonly ICommandAlarmAnalyticsService _analyticsService;

    public AdminController(
        IMediator mediator, 
        IUserActivityService activityService,
        ICommandAlarmAnalyticsService analyticsService)
    {
        _mediator = mediator;
        _activityService = activityService;
        _analyticsService = analyticsService;
    }

    /// <summary>
    /// Retrieves the latest application log file content.
    /// </summary>
    /// <remarks>
    /// Access restricted to users with 'Admin' or 'Moderator' roles.
    /// Searches for the most recently written file matching "app*.log" in the 'logs' directory.
    /// </remarks>
    /// <returns>
    /// A File response containing the log content as "text/plain", or 404 Not Found if the directory or file is missing.
    /// </returns>
    [Authorize(Roles = "Admin,Moderator")]
    [HttpGet("logs")]
    public IActionResult GetLogs()
    {
        var logsDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");

        if (!Directory.Exists(logsDir))
            return NotFound("Logs directory not found");

        var latestLog = Directory.GetFiles(logsDir, "app*.log")
            .OrderByDescending(System.IO.File.GetLastWriteTime)
            .FirstOrDefault();

        if (latestLog == null)
            return NotFound("Log file not found");

        using var stream = new FileStream(
            latestLog,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite
        );

        using var reader = new StreamReader(stream, Encoding.UTF8);
        var content = reader.ReadToEnd();

        var fileName = Path.GetFileName(latestLog);

        return File(Encoding.UTF8.GetBytes(content), "text/plain", fileName);
    }

    /// <summary>
    /// Assigns the 'Moderator' role to a specified user.
    /// </summary>
    /// <remarks>
    /// Access restricted to users with the 'Admin' role.
    /// </remarks>
    /// <param name="userId">The ID of the user to be promoted to Moderator.</param>
    /// <returns>
    /// A 200 OK response on successful role assignment.
    /// </returns>
    [Authorize(Roles = "Admin")]
    [HttpPost("{userId}/make-moderator")]
    public async Task<IActionResult> MakeModerator(long userId)
    {
        await _mediator.Send(new AssignModeratorCommand(userId));
        return Ok("User promoted to Moderator");
    }

    /// <summary>
    /// Removes the 'Moderator' role from a specified user.
    /// </summary>
    /// <remarks>
    /// Access restricted to users with the 'Admin' role.
    /// </remarks>
    /// <param name="userId">The ID of the user whose Moderator role should be revoked.</param>
    /// <returns>
    /// A 200 OK response on successful role removal.
    /// </returns>
    [Authorize(Roles = "Admin")]
    [HttpPost("{userId}/remove-moderator")]
    public async Task<IActionResult> RemoveModerator(long userId)
    {
        await _mediator.Send(new RemoveModeratorCommand(userId));
        return Ok("Moderator role removed");
    }

    /// <summary>
    /// Retrieves a list of all users currently holding the 'Moderator' role.
    /// </summary>
    /// <remarks>
    /// Access restricted to users with the 'Admin' role.
    /// </remarks>
    /// <returns>
    /// A 200 OK response containing a list of moderators.
    /// </returns>
    [Authorize(Roles = "Admin")]
    [HttpGet("moderators")]
    public async Task<IActionResult> GetModerators()
    {
        var list = await _mediator.Send(new GetModeratorsQuery());
        return Ok(list);
    }

    /// <summary>
    /// Retrieves current system-wide configuration settings.
    /// </summary>
    /// <remarks>
    /// Access restricted to users with the 'Admin' role.
    /// </remarks>
    /// <returns>
    /// A 200 OK response containing the system settings.
    /// </returns>
    [Authorize(Roles = "Admin")]
    [HttpGet("system-settings")]
    public async Task<IActionResult> GetSystemSettings()
    {
        var result = await _mediator.Send(new GetSystemSettingsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Updates system-wide configuration settings.
    /// </summary>
    /// <remarks>
    /// Access restricted to users with the 'Admin' role. Allows modification of limits like maximum aquariums, devices, and rules per device.
    /// </remarks>
    /// <param name="request">The request body containing new values for system settings.</param>
    /// <returns>
    /// A 200 OK response confirming successful update.
    /// </returns>
    [Authorize(Roles = "Admin")]
    [HttpPut("update-system-settings")]
    public async Task<IActionResult> UpdateSystemSettings([FromBody] UpdateSystemSettingsRequest request)
    {
        var cmd = new UpdateSystemSettingsCommand(
            MaxAquariumsPerUser: request.MaxAquariumsPerUser,
            MaxDevicesPerAquarium: request.MaxDevicesPerAquarium,
            MaxAlarmRulesPerDevice: request.MaxAlarmRulesPerDevice,
            MaxScheduledCommandsPerDevice: request.MaxScheduledCommandsPerDevice
        );

        await _mediator.Send(cmd);
        return Ok("System settings updated");
    }

    /// <summary>
    /// Retrieves daily activity statistics for the specified number of past days.
    /// </summary>
    /// <remarks>
    /// Access restricted to users with 'Admin' or 'Moderator' roles.
    /// Defaults to returning statistics for the last 7 days.
    /// </remarks>
    /// <param name="days">The number of past days to include in the activity statistics.</param>
    /// <returns>
    /// A 200 OK response with the daily activity statistics data.
    /// </returns>
    [Authorize(Roles = "Admin,Moderator")]
    [HttpGet("daily-activity/{days:int}")]
    public async Task<IActionResult> GetDailyActivity(int days = 7)
    {
        return Ok(await _activityService.GetDailyStatsAsync(days));
    }

    /// <summary>
    /// Analyzes the correlation between executed commands and triggered alarms within a specified time range.
    /// </summary>
    /// <remarks>
    /// Access restricted to users with 'Admin' or 'Moderator' roles. Used for system analytics.
    /// </remarks>
    /// <param name="from">The start date and time for the analysis period (query parameter).</param>
    /// <param name="to">The end date and time for the analysis period (query parameter).</param>
    /// <returns>
    /// A 200 OK response containing the correlation analysis results.
    /// </returns>
    [Authorize(Roles = "Admin,Moderator")]
    [HttpGet("command-alarm-correlation")]
    public async Task<IActionResult> GetCorrelation([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var data = await _analyticsService.AnalyzeAsync(from, to,TimeSpan.FromMinutes(10));

        return Ok(data);
    }
}
