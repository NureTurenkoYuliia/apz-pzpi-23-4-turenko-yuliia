using Application.DTOs.SupportMessages;
using MediatR;

namespace Application.SupportMessages.Queries.GetHistory;

public record GetHistoryOfMessagesQuery(long UserId, long FirstMessageId) : IRequest<List<SupportMessageDto>>;