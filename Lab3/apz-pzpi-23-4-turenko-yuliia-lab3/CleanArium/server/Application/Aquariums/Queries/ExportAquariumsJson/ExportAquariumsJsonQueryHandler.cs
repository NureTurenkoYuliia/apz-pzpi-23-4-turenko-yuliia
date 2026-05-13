using Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Aquariums.Queries.ExportAquariumsJson;

public class ExportAquariumsJsonQueryHandler : IRequestHandler<ExportAquariumsJsonQuery, byte[]>
{
    private readonly IAquariumRepository _repo;
    private readonly IJsonExportService _jsonExportService;
    private readonly ILogger<ExportAquariumsJsonQueryHandler> _logger;

    public ExportAquariumsJsonQueryHandler(
        IAquariumRepository repo,
        IJsonExportService jsonExportService,
        ILogger<ExportAquariumsJsonQueryHandler> logger)
    {
        _repo = repo;
        _jsonExportService = jsonExportService;
        _logger = logger;
    }

    public async Task<byte[]> Handle(ExportAquariumsJsonQuery request, CancellationToken ct)
    {
        _logger.LogInformation("USER_ACTION Exporting aquariums for User {UserId}", request.UserId);

        var aquariums = await _repo.GetAllByUserIdAsync(request.UserId);

        return _jsonExportService.Export(aquariums);
    }
}