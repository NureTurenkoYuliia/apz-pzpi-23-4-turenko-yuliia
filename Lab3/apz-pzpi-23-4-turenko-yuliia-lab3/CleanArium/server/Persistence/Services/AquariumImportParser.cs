using Application.Abstractions;
using Application.DTOs.Aquariums;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Persistence.Services;

public class AquariumImportParser : IImportParser<AquariumImportDto>
{
    public async Task<List<AquariumImportDto>> ParseJsonAsync(IFormFile file)
    {
        using var stream = new StreamReader(file.OpenReadStream());
        var json = await stream.ReadToEndAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        return JsonSerializer.Deserialize<List<AquariumImportDto>>(json, options)!;
    }

    public async Task<List<AquariumImportDto>> ParseCsvAsync(IFormFile file)
    {
        var list = new List<AquariumImportDto>();
        using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
        string? line;
        reader.ReadLine();

        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var p = line.Split(',')
                        .Select(x => x.Trim().Trim('"'))
                        .ToArray();

            if (p.Length < 3)
                continue;

            var dto = new AquariumImportDto
            {
                Name = p[0],
                Location = p[1],
                Devices = new List<DeviceImportDto>()
            };

            var devicesRaw = p[2].Split(';', StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim().Trim('"'))
                                .ToArray();

            foreach (var d in devicesRaw)
            {
                var devParts = d.Split('|', StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim().Trim('"'))
                                .ToArray();

                if (devParts.Length != 2)
                    continue;

                dto.Devices.Add(new DeviceImportDto
                {
                    DeviceType = Enum.Parse<DeviceType>(devParts[0], ignoreCase: true),
                    DeviceStatus = Enum.Parse<DeviceStatus>(devParts[1], ignoreCase: true)
                });
            }

            list.Add(dto);
        }

        return list;
    }
}
