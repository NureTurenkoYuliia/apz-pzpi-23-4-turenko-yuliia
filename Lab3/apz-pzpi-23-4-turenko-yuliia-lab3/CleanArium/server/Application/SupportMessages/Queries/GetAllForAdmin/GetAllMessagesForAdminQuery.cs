using Application.DTOs.SupportMessages;
using MediatR;

namespace Application.SupportMessages.Queries.GetAllForAdmin;

public class GetAllMessagesForAdminQuery() : IRequest<List<PreviewSupportMessageDto>>;