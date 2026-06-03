using Application.Abstractions;
using Application.DTOs.AlarmRules;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Persistence.Services;

public class AlarmRuleImportParser : IImportParser<AlarmRuleImportDto>
{
    public async Task<List<AlarmRuleImportDto>> ParseJsonAsync(IFormFile file)
    {
        using var stream = new StreamReader(file.OpenReadStream());
        var json = await stream.ReadToEndAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        return JsonSerializer.Deserialize<List<AlarmRuleImportDto>>(json, options)!;
    }

    public async Task<List<AlarmRuleImportDto>> ParseCsvAsync(IFormFile file)
    {
        var list = new List<AlarmRuleImportDto>();
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

            list.Add(new AlarmRuleImportDto
            {
                Condition = Enum.Parse<ConditionType>(p[0], ignoreCase: true),
                Threshold = float.Parse(p[1], CultureInfo.InvariantCulture),
                Unit = p[2]
            });
        }

        return list;
    }
}