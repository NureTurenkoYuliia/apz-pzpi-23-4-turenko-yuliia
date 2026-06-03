using MediatR;

namespace Application.Notifications.Commands.MarkAsRead;

public record MarkAsReadCommand (long UserId, long NotificationId) : IRequest;