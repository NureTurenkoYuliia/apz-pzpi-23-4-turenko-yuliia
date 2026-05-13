namespace Application.Abstractions;

public interface IJsonExportService
{
    byte[] Export(object data);
}