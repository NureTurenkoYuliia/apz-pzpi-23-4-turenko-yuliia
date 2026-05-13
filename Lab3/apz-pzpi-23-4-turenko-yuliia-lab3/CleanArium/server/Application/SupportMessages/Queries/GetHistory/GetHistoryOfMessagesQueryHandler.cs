using Application.Abstractions;
using Application.DTOs.SupportMessages;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.SupportMessages.Queries.GetHistory;

public class GetHistoryOfMessagesQueryHandler : IRequestHandler<GetHistoryOfMessagesQuery, List<SupportMessageDto>>
{
    private readonly ISupportMessageRepository _repo;
    private readonly ILogger<GetHistoryOfMessagesQueryHandler> _logger;

    public GetHistoryOfMessagesQueryHandler(ISupportMessageRepository repo, ILogger<GetHistoryOfMessagesQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<List<SupportMessageDto>> Handle(GetHistoryOfMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _repo.GetHistoryAsync(request.FirstMessageId);

        List<SupportMessageDto> list = messages.Select(m => new SupportMessageDto
        {
            Id = m.Id,
            UserId = m.UserId,
            ReplyToMessageId = m.ReplyToMessageId,
            Subject = m.Subject,
            Message = m.Message,
            MessageStatus = m.MessageStatus,
            CreatedAt = m.CreatedAt
        })
        .ToList();

        _logger.LogInformation("USER_ACTION Successfully retrieved history of messages where first: {Id} ", request.FirstMessageId);

        return list;
    }
}