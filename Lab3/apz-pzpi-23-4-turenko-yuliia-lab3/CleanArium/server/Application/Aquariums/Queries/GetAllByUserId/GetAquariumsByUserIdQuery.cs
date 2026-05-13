using Application.DTOs.Aquariums;
using MediatR;

namespace Application.Aquariums.Queries.GetAllByUserId;

public record GetAquariumsByUserIdQuery(long UserId) : IRequest<List<AquariumDto>>;