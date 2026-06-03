using Microsoft.AspNetCore.Http;

namespace Application.Abstractions;

public interface IImportParser<T>
{
    Task<List<T>> ParseJsonAsync(IFormFile file);
    Task<List<T>> ParseCsvAsync(IFormFile file);
}