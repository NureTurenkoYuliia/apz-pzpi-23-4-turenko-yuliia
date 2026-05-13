using Application.DTOs.Users;
using MediatR;

namespace Application.Users.Queries.GetAll;

public record GetAllUsersQuery() : IRequest<List<PreviewUserDto>>;