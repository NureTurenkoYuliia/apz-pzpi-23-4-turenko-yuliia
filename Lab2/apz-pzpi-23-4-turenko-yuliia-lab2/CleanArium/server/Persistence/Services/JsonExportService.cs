using Application.Abstractions;
using System.Text;

namespace Persistence.Services;

public class JsonExportService : IJsonExportService
{
    public byte[] Export(object data)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(
            data,
            new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

        return Encoding.UTF8.GetBytes(json);
    }
}
