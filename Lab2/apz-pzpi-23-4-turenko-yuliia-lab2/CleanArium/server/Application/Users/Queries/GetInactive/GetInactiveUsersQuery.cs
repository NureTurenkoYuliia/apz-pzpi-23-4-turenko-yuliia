using Application.DTOs.Users;
using MediatR;

namespace Application.Users.Queries.GetInactive;

public record GetInactiveUsersQuery(int limitDays) : IRequest<List<InactiveUserDto>>;
