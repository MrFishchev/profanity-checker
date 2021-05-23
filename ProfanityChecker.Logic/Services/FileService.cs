#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace ProfanityChecker.Logic
{
    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;

        public FileService(ILogger<FileService> logger)
        {
            _logger = logger;
        }
        
        public async Task<string?> SaveAsTempFileAsync(IFormFile file, CancellationToken ct)
        {
            try
            {
                if (file.Length == 0) return null;
                
                var tempPath = Path.GetTempFileName();
                await using var fs = new FileStream(tempPath, FileMode.Truncate);
                await file.CopyToAsync(fs, ct);
                return tempPath;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to save file into a temp folder");
                return null;
            }
        }

        public Task DeleteFileAsync(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch(Exception e)
            {
                _logger.LogError(e, $"Unable to delete file {path}");
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
                _logger.LogError(e, $"Unable to get whole text of the file ({path})");
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
            catch (OperationCanceledException e)
            {
                _logger.LogInformation(e,$"Operation was cancelled {nameof(GetLinesAsync)}");
                return new List<string>(0);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unable to get lines from file {path}");
                return result;
            }

            return result;
        }
    }
}