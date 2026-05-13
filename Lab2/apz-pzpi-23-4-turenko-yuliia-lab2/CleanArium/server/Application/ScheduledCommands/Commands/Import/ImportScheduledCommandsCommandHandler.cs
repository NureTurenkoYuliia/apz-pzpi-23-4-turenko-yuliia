using Application.Abstractions;
using Application.DTOs.Result;
using Application.DTOs.ScheduledCommands;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.ScheduledCommands.Commands.Import;

public class ImportScheduledCommandsCommandHandler : IRequestHandler<ImportScheduledCommandsCommand, ImportResult>
{
    private readonly IDeviceRepository _deviceRepo;
    private readonly IScheduledCommandRepository _scheduledRepo;
    private readonly ISystemSettingsRepository _settingsRepo;
    private readonly IImportParser<ScheduledCommandImportDto> _parser;
    private readonly ILogger<ImportScheduledCommandsCommandHandler> _logger;

    public ImportScheduledCommandsCommandHandler(
        IDeviceRepository deviceRepo,
        IScheduledCommandRepository scheduledRepo,
        ISystemSettingsRepository settingsRepo,
        IImportParser<ScheduledCommandImportDto> parser,
        ILogger<ImportScheduledCommandsCommandHandler> logger)
    {
        _deviceRepo = deviceRepo;
        _scheduledRepo = scheduledRepo;
        _settingsRepo = settingsRepo;
        _parser = parser;
        _logger = logger;
    }

    public async Task<ImportResult> Handle(ImportScheduledCommandsCommand request, CancellationToken ct)
    {
        var device = await _deviceRepo.GetByIdAsync(request.DeviceId);
        if (device == null) throw new Exception("Device not found");

        var isJson = request.File.FileName.EndsWith(".json");
        var data = isJson
            ? await _parser.ParseJsonAsync(request.File)
            : await _parser.ParseCsvAsync(request.File);

        int count = 0;

        var existing = await _scheduledRepo.CountByDevice(request.UserId, ct);
        var settings = await _settingsRepo.GetAsync(ct);

        while (existing + count < settings.MaxScheduledCommandsPerDevice)
        { 
        
        }
            foreach (var dto in data)
        {
            var sc = new ScheduledCommand
            {
                DeviceId = request.DeviceId,
                CommandType = dto.CommandType,
                StartTime = dto.StartTime,
                RepeatMode = dto.RepeatMode,
                IntervalMinutes = dto.IntervalMinutes,
                IsActive = dto.IsActive
            };

            await _scheduledRepo.AddAsync(sc);
            count++;
        }

        _logger.LogInformation("USER_ACTION Imported {Count} aquariums with devices for user {User}", count, request.UserId);

        return new ImportResult { ImportedCount = count };
    }
}