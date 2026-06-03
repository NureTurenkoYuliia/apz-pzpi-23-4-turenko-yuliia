using MediatR;

namespace Application.Notifications.Queries.GetUnreadCount;

public record GetUnreadNotificationsCountQuery(long UserId) : IRequest<int>;