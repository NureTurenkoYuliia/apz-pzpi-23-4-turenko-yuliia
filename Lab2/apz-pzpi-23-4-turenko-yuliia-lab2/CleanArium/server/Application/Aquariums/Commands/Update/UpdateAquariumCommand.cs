using MediatR;

namespace Application.Aquariums.Commands.Update;

public record UpdateAquariumCommand(long UserId, long AquariumId, string Name, string? Location) : IRequest;