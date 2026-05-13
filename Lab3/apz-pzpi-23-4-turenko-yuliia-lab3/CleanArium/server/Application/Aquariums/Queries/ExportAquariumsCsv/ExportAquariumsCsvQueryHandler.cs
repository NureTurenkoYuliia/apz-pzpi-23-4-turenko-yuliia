using Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Application.Aquariums.Queries.ExportAquariumsCsv;

public class ExportAquariumsCsvQueryHandler : IRequestHandler<ExportAquariumsCsvQuery, byte[]>
{
    private readonly IAquariumRepository _repo;
    private readonly ILogger<ExportAquariumsCsvQueryHandler> _logger;

    public ExportAquariumsCsvQueryHandler(IAquariumRepository repo, ILogger<ExportAquariumsCsvQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<byte[]> Handle(ExportAquariumsCsvQuery request, CancellationToken ct)
    {
        _logger.LogInformation("USER_ACTION Exporting aquariums for User {UserId}", request.UserId);

        var aquariums = await _repo.GetAllByUserIdAsync(request.UserId);

        var sb = new StringBuilder();

        sb.AppendLine("AquariumId,Name,Location,IsActive,CreatedAt,DevicesCount");

        foreach (var a in aquariums)
        {
            var line = $"{a.Id}," +
                       $"{Escape(a.Name)}," +
                       $"{Escape(a.Location)}," +
                       $"{a.IsActive}," +
                       $"{a.CreatedAt:yyyy-MM-dd}," +
                       $"{a.Devices?.Count ?? 0}";

            sb.AppendLine(line);
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static string Escape(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return "";
        return value.Replace(",", ";");
    }
}
