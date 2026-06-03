using Application.DTOs.Users;
using MediatR;

namespace Application.Users.Queries.GetModerators;

public record GetModeratorsQuery() : IRequest<List<ModeratorDto>>;