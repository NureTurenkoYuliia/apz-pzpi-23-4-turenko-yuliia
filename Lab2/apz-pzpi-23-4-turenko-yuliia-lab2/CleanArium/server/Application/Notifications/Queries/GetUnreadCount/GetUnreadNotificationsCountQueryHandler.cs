using Application.Abstractions;
using MediatR;
namespace Application.Notifications.Queries.GetUnreadCount;

public class GetUnreadNotificationsCountQueryHandler : IRequestHandler<GetUnreadNotificationsCountQuery, int>
{
    private readonly INotificationRepository _repo;

    public GetUnreadNotificationsCountQueryHandler(INotificationRepository repo)
    {
        _repo = repo;
    }

    public async Task<int> Handle(GetUnreadNotificationsCountQuery request, CancellationToken cancellationToken)
    {
        return await _repo.CountUnreadAsync(request.UserId);
    }
}