using Application.Abstractions;
using Application.DTOs.Aquariums;
using Application.DTOs.Result;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Aquariums.Commands.Import;

public class ImportAquariumsWithDevicesCommandHandler : IRequestHandler<ImportAquariumsWithDevicesCommand, ImportResult>
{
    private readonly IAquariumRepository _aqRepo;
    private readonly IDeviceRepository _deviceRepo;
    private readonly ISystemSettingsRepository _settingsRepo;
    private readonly IImportParser<AquariumImportDto> _parser;
    private readonly ILogger<ImportAquariumsWithDevicesCommandHandler> _logger;

    public ImportAquariumsWithDevicesCommandHandler(
        IAquariumRepository aqRepo,
        IDeviceRepository deviceRepo,
        ISystemSettingsRepository settingsRepo,
        IImportParser<AquariumImportDto> parser,
        ILogger<ImportAquariumsWithDevicesCommandHandler> logger)
    {
        _aqRepo = aqRepo;
        _deviceRepo = deviceRepo;
        _settingsRepo = settingsRepo;
        _parser = parser;
        _logger = logger;
    }

    public async Task<ImportResult> Handle(ImportAquariumsWithDevicesCommand request, CancellationToken ct)
    {
        var isJson = request.File.FileName.EndsWith(".json");
        var data = isJson
            ? await _parser.ParseJsonAsync(request.File)
            : await _parser.ParseCsvAsync(request.File);

        int count = 0;
        int deviceСount = 0;

        var existing = await _aqRepo.CountByUser(request.UserId, ct);
        var settings = await _settingsRepo.GetAsync(ct);

        while(existing + count < settings.MaxAquariumsPerUser)
        {
            foreach (var dto in data)
            {
                var aquarium = new Aquarium
                {
                    UserId = request.UserId,
                    Name = dto.Name,
                    Location = dto.Location,
                };
            
                await _aqRepo.AddAsync(aquarium);

                while (count < settings.MaxDevicesPerAquarium)
                {
                    foreach (var dev in dto.Devices)
                    {
                        var device = new Device
                        {
                            AquariumId = aquarium.Id,
                            DeviceType = dev.DeviceType,
                            DeviceStatus = dev.DeviceStatus
                        };

                        await _deviceRepo.AddAsync(device);
                        deviceСount++;
                    }

                    count++;
                }
            }
        }

        _logger.LogInformation("USER_ACTION Imported {Count} aquariums with {deviceСount} devices for user {User}", count, deviceСount, request.UserId);

        return new ImportResult { ImportedCount = count };
    }
}

