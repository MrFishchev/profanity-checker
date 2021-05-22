#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

// ReSharper disable once CheckNamespace
namespace ProfanityChecker.Logic
{
    public interface IFileService
    {
        Task<string?> SaveAsTempFileAsync(IFormFile file, CancellationToken ct = default);

        Task DeleteFileAsync(string path);

        Task<string?> GetWholeTextAsync(string path);

        Task<IEnumerable<string>> GetLinesAsync(string path, CancellationToken ct = default);
    }
}