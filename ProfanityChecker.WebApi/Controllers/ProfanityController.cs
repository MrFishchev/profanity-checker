using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProfanityChecker.Logic;
using ProfanityChecker.Logic.DTO;

namespace ProfanityChecker.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfanityController : ControllerBase
    {
        private readonly IProfanityService _profanityService;
        private readonly IFileService _fileService;

        public ProfanityController(IProfanityService profanityService, IFileService fileService,
            ILogger<ProfanityController> logger)
        {
            _profanityService = profanityService;
            _fileService = fileService;
        }

        [HttpPost]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Create))]
        public Task<ActionResult<ProfanityScanResultDto>> FindAllProfanity([FromForm]IFormFile file, CancellationToken ct = default)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file), "Parameter cannot be null");
            
            if (file.Length == 0)
                throw new ArgumentException("File cannot be empty", nameof(file));

            return FindAllAsync();

            async Task<ActionResult<ProfanityScanResultDto>> FindAllAsync()
            {
                var tempPath = await _fileService.SaveAsTempFileAsync(file, ct);
                if (string.IsNullOrWhiteSpace(tempPath))
                    return Problem("Unable to save a file");

                var fileData = await _fileService.GetWholeTextAsync(tempPath);
                if (string.IsNullOrWhiteSpace(fileData))
                    return NoContent();

                var result = await _profanityService.ScanAsync(fileData, ct);
                await _fileService.DeleteFileAsync(tempPath);

                return new ProfanityScanResultDto(result.HasProfanity, result.ProfanityItems);
            }
        }
    }
}