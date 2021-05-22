#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

// ReSharper disable once CheckNamespace
namespace ProfanityChecker.Logic
{
    public class FileService : IFileService
    {
        public async Task<string?> SaveAsTempFileAsync(IFormFile file, CancellationToken ct)
        {
            try
            {
                var tempPath = Path.GetTempFileName();
                await using var fs = new FileStream(tempPath, FileMode.Truncate);
                await file.CopyToAsync(fs, ct);
                return tempPath;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Task DeleteFileAsync(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch
            {
                // ignore    
            }
            
            return Task.CompletedTask;
        }

        public async Task<string?> GetWholeTextAsync(string path)
        {
            try
            {
                using var sr = new StreamReader(path);
                return await sr.ReadToEndAsync();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<IEnumerable<string>> GetLinesAsync(string path, CancellationToken ct)
        {
            var result = new List<string>();
            try
            {
                using var sr = new StreamReader(path);
                while (!sr.EndOfStream)
                {
                    ct.ThrowIfCancellationRequested();
                    var line = await sr.ReadLineAsync();
                    if (!string.IsNullOrWhiteSpace(line))
                        result.Add(line);
                }
            }
            catch (Exception e)
            {
                return result;
            }

            return result;
        }
    }
}