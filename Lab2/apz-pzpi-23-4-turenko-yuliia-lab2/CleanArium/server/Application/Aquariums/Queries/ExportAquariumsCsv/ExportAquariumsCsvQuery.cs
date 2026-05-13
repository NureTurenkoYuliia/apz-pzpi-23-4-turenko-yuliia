using MediatR;

namespace Application.Aquariums.Queries.ExportAquariumsCsv;

public record ExportAquariumsCsvQuery(long UserId) : IRequest<byte[]>;
