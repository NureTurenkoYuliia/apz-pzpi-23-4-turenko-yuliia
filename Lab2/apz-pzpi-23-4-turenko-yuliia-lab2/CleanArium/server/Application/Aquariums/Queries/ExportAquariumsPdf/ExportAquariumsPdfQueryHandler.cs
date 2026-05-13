using Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Aquariums.Queries.ExportAquariumsPdf;

public class ExportAquariumsPdfQueryHandler : IRequestHandler<ExportAquariumsPdfQuery, byte[]>
{
    private readonly IAquariumRepository _repo;
    private readonly IPdfExportService _pdfExportService;
    private readonly ILogger<ExportAquariumsPdfQueryHandler> _logger;

    public ExportAquariumsPdfQueryHandler(
        IAquariumRepository repo,
        IPdfExportService pdfExportService,
        ILogger<ExportAquariumsPdfQueryHandler> logger)
    {
        _repo = repo;
        _pdfExportService = pdfExportService;
        _logger = logger;
    }

    public async Task<byte[]> Handle(ExportAquariumsPdfQuery request, CancellationToken ct)
    {
        _logger.LogInformation("USER_ACTION Exporting aquariums for User {UserId}", request.UserId);

        var aquariums = await _repo.GetAllWithDevicesByUserIdAsync(request.UserId);

        return _pdfExportService.ExportAquariums(aquariums);
    }
}
