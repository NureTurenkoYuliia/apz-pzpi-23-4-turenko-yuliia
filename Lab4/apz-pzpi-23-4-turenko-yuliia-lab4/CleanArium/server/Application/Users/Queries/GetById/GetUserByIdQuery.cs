using Application.DTOs.Users;
using MediatR;

namespace Application.Users.Queries.GetById;

public record GetUserByIdQuery(long UserId) : IRequest<UserDto>;