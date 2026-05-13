using MediatR;

namespace Application.Aquariums.Commands.Delete;

public record DeleteAquariumCommand(long UserId, long AquariumId) : IRequest;