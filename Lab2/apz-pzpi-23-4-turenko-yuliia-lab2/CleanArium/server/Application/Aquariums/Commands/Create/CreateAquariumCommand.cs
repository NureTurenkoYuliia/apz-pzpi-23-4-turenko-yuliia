using MediatR;

namespace Application.Aquariums.Commands.Create;

public record CreateAquariumCommand(long UserId, string Name, string? Location) : IRequest<long>;
