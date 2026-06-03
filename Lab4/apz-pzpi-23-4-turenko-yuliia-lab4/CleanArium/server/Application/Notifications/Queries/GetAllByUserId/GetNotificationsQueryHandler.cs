using Application.Abstractions;
using Application.DTOs.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Notifications.Queries.GetAllByUserId;

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, List<NotificationDto>>
{
    private readonly INotificationRepository _repo;
    private readonly ILogger<GetNotificationsQueryHandler> _logger;

    public GetNotificationsQueryHandler(INotificationRepository repo, ILogger<GetNotificationsQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<List<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var userNotifications = await _repo.GetUserNotificationsAsync(request.UserId);

        List<NotificationDto> list = userNotifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Title = n.Title,
            Content = n.Content,
            CreatedAt = n.CreatedAt,
            IsRead = n.IsRead
        })
        .ToList();

        _logger.LogInformation("Successfully retrieved notifications for user: {Id} ", request.UserId);

        return list;
    }
}
