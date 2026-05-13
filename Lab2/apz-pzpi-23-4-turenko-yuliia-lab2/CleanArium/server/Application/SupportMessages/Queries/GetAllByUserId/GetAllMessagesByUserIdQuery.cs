using Application.DTOs.SupportMessages;
using MediatR;

namespace Application.SupportMessages.Queries.GetAllByUserId;

public record GetAllMessagesByUserIdQuery(long UserId) : IRequest<List<PreviewSupportMessageDto>>;