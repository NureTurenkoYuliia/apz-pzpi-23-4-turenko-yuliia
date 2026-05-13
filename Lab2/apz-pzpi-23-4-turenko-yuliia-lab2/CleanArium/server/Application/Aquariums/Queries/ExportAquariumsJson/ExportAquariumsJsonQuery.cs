using MediatR;

namespace Application.Aquariums.Queries.ExportAquariumsJson;

public record ExportAquariumsJsonQuery(long UserId) : IRequest<byte[]>;