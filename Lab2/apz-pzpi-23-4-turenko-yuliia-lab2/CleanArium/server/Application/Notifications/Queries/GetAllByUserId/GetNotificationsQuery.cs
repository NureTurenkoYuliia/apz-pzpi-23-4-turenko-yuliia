using Application.DTOs.Notifications;
using MediatR;

namespace Application.Notifications.Queries.GetAllByUserId;

public record GetNotificationsQuery(long UserId) : IRequest<List<NotificationDto>>;