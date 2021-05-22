using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProfanityChecker.Domain;
using ProfanityChecker.Infrastructure;
using ProfanityChecker.Logic;
using ProfanityChecker.Logic.DTO;
using ProfanityChecker.WebApi.Requests;

namespace ProfanityChecker.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BannedDictionaryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public BannedDictionaryController(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        [HttpGet]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.GetAll))]
        public async Task<IEnumerable<BannedPhraseDto>> GetAll(CancellationToken ct = default)
        {
            var bannedPhrases = await _unitOfWork.BannedPhrases.GetAllAsync(ct);
            return bannedPhrases.Select(x => new BannedPhraseDto {Id = x.Id, Name = x.Name});
        }

        [HttpGet("{id:long}")]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.GetBy))]
        public async Task<ActionResult<BannedPhraseDto>> GetById(long id, CancellationToken ct = default)
        {
            var bannedPhrase = await _unitOfWork.BannedPhrases.GetByIdAsync(id, ct);

            if (bannedPhrase == null)
                return NotFound();

            return new BannedPhraseDto {Id = bannedPhrase.Id, Name = bannedPhrase.Name};
        }

        [HttpPost]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Create))]
        public Task<ActionResult<long>> Create(AddBannedPhraseRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Parameter cannot be null or empty", nameof(request.Name));

            return CreateAsync();

            async Task<ActionResult<long>> CreateAsync()
            {
                var entity = await _unitOfWork.BannedPhrases.AddAsync(new BannedPhrase {Name = request.Name}, ct);
                return entity is null ? Conflict() : CreatedAtAction(nameof(Create), entity.Id);
            }
        }
        
        [HttpPost("range")]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Create))]
        public Task<ActionResult<int>> CreateRangeFromFile([FromForm]IFormFile file, CancellationToken ct = default)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file), "Parameter cannot be null");
            
            if (file.Length == 0)
                throw new ArgumentException("File cannot be empty", nameof(file));

            return CreateRangeAsync();

            async Task<ActionResult<int>> CreateRangeAsync()
            {
                var tempPath = await _fileService.SaveAsTempFileAsync(file, ct);
                if (string.IsNullOrWhiteSpace(tempPath))
                    return Problem("Unable to save a file");

                var existingPhrases = (await _unitOfWork.BannedPhrases.GetAllAsync(ct)).ToList();
                var fileLines = (await _fileService.GetLinesAsync(tempPath)).ToList();

                var addedPhraseCount = 0;
                foreach (var line in fileLines)
                {
                    if (!string.IsNullOrWhiteSpace(line) && existingPhrases.All(x => x.Name != line))
                    {
                        await _unitOfWork.BannedPhrases.AddAsync(new BannedPhrase {Name = line}, ct);
                        addedPhraseCount++;
                    }
                }
                
                await _unitOfWork.SaveChangesAsync(ct);
                await _fileService.DeleteFileAsync(tempPath);
                return CreatedAtAction(nameof(CreateRangeFromFile), addedPhraseCount);
            }
        }

        [HttpDelete("{id:long}")]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Delete))]
        public async Task<ActionResult> Delete(long id, CancellationToken ct = default)
        {
            var bannedPhrase = await _unitOfWork.BannedPhrases.GetByIdAsync(id, ct);
            if (bannedPhrase == null) return NotFound();
            
            await _unitOfWork.BannedPhrases.DeleteAsync(bannedPhrase);
            await _unitOfWork.SaveChangesAsync(ct);
            return Ok();
        }
    }
}