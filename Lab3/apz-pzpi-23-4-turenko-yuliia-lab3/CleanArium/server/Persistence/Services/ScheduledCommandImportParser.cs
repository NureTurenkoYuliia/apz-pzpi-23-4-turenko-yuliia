using Application.Abstractions;
using Application.DTOs.ScheduledCommands;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Persistence.Services;

public class ScheduledCommandImportParser : IImportParser<ScheduledCommandImportDto>
{
    public async Task<List<ScheduledCommandImportDto>> ParseJsonAsync(IFormFile file)
    {
        using var stream = new StreamReader(file.OpenReadStream());
        var json = await stream.ReadToEndAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        return JsonSerializer.Deserialize<List<ScheduledCommandImportDto>>(json, options)!;
    }

    public async Task<List<ScheduledCommandImportDto>> ParseCsvAsync(IFormFile file)
    {
        var list = new List<ScheduledCommandImportDto>();
        using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
        string? line;
        reader.ReadLine(); // skip header

        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var p = line.Split(';')
                        .Select(x => x.Trim().Trim('"'))
                        .ToArray();

            list.Add(new ScheduledCommandImportDto
            {
                CommandType = Enum.Parse<CommandType>(p[0], ignoreCase: true),
                StartTime = DateTime.Parse(p[1]),
                RepeatMode = Enum.Parse<RepeatMode>(p[2], ignoreCase: true),
                IntervalMinutes = string.IsNullOrEmpty(p[3]) ? null : int.Parse(p[3]),
                IsActive = bool.Parse(p[4])
            });
        }
        return list;
    }
}