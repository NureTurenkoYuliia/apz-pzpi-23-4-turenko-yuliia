using Application.Abstractions;
using Application.DTOs.SupportMessages;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.SupportMessages.Queries.GetAllForAdmin;

public class GetAllMessagesForAdminQueryHandler : IRequestHandler<GetAllMessagesForAdminQuery, List<PreviewSupportMessageDto>>
{
    private readonly ISupportMessageRepository _repo;
    private readonly ILogger<GetAllMessagesForAdminQueryHandler> _logger;

    public GetAllMessagesForAdminQueryHandler(ISupportMessageRepository repo, ILogger<GetAllMessagesForAdminQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<List<PreviewSupportMessageDto>> Handle(GetAllMessagesForAdminQuery request, CancellationToken cancellationToken)
    {
        var messages = await _repo.GetAllForAdminAsync();

        List<PreviewSupportMessageDto> list = messages.Select(m => new PreviewSupportMessageDto
        {
            Id = m.Id,
            Subject = m.Subject,
            CreatedAt = m.CreatedAt
        })
        .ToList();

        _logger.LogInformation("Successfully retrieved messages for Admin.");

        return list;
    }
}