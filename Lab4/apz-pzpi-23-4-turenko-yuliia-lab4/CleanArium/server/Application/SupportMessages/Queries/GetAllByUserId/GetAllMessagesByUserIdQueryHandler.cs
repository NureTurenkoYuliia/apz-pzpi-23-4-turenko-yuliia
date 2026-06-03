using Application.Abstractions;
using Application.DTOs.SupportMessages;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.SupportMessages.Queries.GetAllByUserId;

public class GetAllMessagesByUserIdQueryHandler : IRequestHandler<GetAllMessagesByUserIdQuery, List<PreviewSupportMessageDto>>
{
    private readonly ISupportMessageRepository _repo;
    private readonly ILogger<GetAllMessagesByUserIdQueryHandler> _logger;

    public GetAllMessagesByUserIdQueryHandler(ISupportMessageRepository repo, ILogger<GetAllMessagesByUserIdQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<List<PreviewSupportMessageDto>> Handle(GetAllMessagesByUserIdQuery request, CancellationToken cancellationToken)
    {
        var messages = await _repo.GetAllByUserIdAsync(request.UserId);

        List<PreviewSupportMessageDto> list = messages.Select(m => new PreviewSupportMessageDto
        {
            Id = m.Id,
            Subject = m.Subject,
            CreatedAt = m.CreatedAt
        })
        .ToList();

        _logger.LogInformation("USER_ACTION Successfully retrieved messages for User: {Id} ", request.UserId);

        return list;
    }
}