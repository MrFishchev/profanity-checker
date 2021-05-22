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
        /// <summary>
        /// Saves file to a system temp folder with a random name
        /// </summary>
        /// <param name="file">File from HTTP form</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Path to created file or null (if not created)</returns>
        Task<string?> SaveAsTempFileAsync(IFormFile file, CancellationToken ct = default);

        /// <summary>
        /// Deletes file by path
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <returns></returns>
        Task DeleteFileAsync(string path);

        /// <summary>
        /// Gets a whole text from a file
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <returns>Whole data of file or null if cannot read</returns>
        Task<string?> GetWholeTextAsync(string path);

        /// <summary>
        /// Gets list of lines of a file
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of lines or empty list if cannot read or cancelled</returns>
        Task<IEnumerable<string>> GetLinesAsync(string path, CancellationToken ct = default);
    }
}