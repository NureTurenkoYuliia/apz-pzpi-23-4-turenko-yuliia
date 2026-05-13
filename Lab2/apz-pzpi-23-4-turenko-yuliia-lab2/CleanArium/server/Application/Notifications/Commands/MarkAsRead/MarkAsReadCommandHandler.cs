using Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Notifications.Commands.MarkAsRead;

public class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand>
{
    private readonly INotificationRepository _repo;
    private readonly ILogger<MarkAsReadCommandHandler> _logger;

    public MarkAsReadCommandHandler(INotificationRepository repo, ILogger<MarkAsReadCommandHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
    {
        var notifications = await _repo.GetUserNotificationsAsync(request.UserId);
        var n = notifications.FirstOrDefault(x => x.Id == request.NotificationId);

        await _repo.MarkAsReadAsync(request.NotificationId);

        _logger.LogInformation("Successfully marked as Read Notification: {Id} ", request.NotificationId);
    }
}