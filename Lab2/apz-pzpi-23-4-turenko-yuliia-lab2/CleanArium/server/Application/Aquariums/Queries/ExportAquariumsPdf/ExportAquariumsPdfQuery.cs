using MediatR;

namespace Application.Aquariums.Queries.ExportAquariumsPdf;

public record ExportAquariumsPdfQuery(long UserId) : IRequest<byte[]>;
