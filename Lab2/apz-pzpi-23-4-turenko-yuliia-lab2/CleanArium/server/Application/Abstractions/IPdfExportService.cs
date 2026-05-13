using Domain.Models;

namespace Application.Abstractions;

public interface IPdfExportService
{
    byte[] ExportAquariums(IEnumerable<Aquarium> data);
}
