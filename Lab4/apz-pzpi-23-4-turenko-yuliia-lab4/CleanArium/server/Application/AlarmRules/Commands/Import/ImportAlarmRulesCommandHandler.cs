using Application.Abstractions;
using Application.DTOs.AlarmRules;
using Application.DTOs.Result;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.AlarmRules.Commands.Import;

public class ImportAlarmRulesCommandHandler : IRequestHandler<ImportAlarmRulesCommand, ImportResult>
{
    private readonly IDeviceRepository _deviceRepo;
    private readonly IAlarmRuleRepository _alarmRepo;
    private readonly ISystemSettingsRepository _settingsRepo;
    private readonly IImportParser<AlarmRuleImportDto> _parser;
    private readonly ILogger<ImportAlarmRulesCommandHandler> _logger;

    public ImportAlarmRulesCommandHandler(
        IDeviceRepository deviceRepo,
        IAlarmRuleRepository alarmRepo,
        ISystemSettingsRepository settingsRepo,
        IImportParser<AlarmRuleImportDto> parser,
        ILogger<ImportAlarmRulesCommandHandler> logger)
    {
        _deviceRepo = deviceRepo;
        _alarmRepo = alarmRepo;
        _settingsRepo = settingsRepo;
        _parser = parser;
        _logger = logger;
    }

    public async Task<ImportResult> Handle(ImportAlarmRulesCommand request, CancellationToken ct)
    {
        var device = await _deviceRepo.GetByIdAsync(request.DeviceId);
        if (device == null) throw new Exception("Device not found");

        var isJson = request.File.FileName.EndsWith(".json");
        var data = isJson
            ? await _parser.ParseJsonAsync(request.File)
            : await _parser.ParseCsvAsync(request.File);

        int count = 0;

        var existing = await _alarmRepo.CountByDevice(request.DeviceId, ct);
        var settings = await _settingsRepo.GetAsync(ct);

        while (existing + count < settings.MaxAquariumsPerUser)
        {
            foreach (var dto in data)
            {
                var rule = new AlarmRule
                {
                    DeviceId = request.DeviceId,
                    Condition = dto.Condition,
                    Threshold = dto.Threshold,
                    Unit = dto.Unit
                };

                await _alarmRepo.AddAsync(rule, ct);
                count++;
            }
        }

        _logger.LogInformation("USER_ACTION Imported {Count} alarm rules for device {Device}", count, request.DeviceId);

        return new ImportResult { ImportedCount = count };
    }
}