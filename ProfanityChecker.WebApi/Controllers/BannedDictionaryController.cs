using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProfanityChecker.Domain;
using ProfanityChecker.Infrastructure;
using ProfanityChecker.Logic.DTO;
using ProfanityChecker.WebApi.Requests;

namespace ProfanityChecker.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BannedDictionaryController : ControllerBase
    {
        private readonly IBannedPhraseRepository _bannedPhraseRepository;

        public BannedDictionaryController(IBannedPhraseRepository bannedPhraseRepository)
        {
            _bannedPhraseRepository = bannedPhraseRepository;
        }

        [HttpGet]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.GetAll))]
        public async Task<IEnumerable<BannedPhraseDto>> GetAll(CancellationToken ct = default)
        {
            var bannedPhrases = await _bannedPhraseRepository.GetAllAsync();
            return bannedPhrases.Select(x => new BannedPhraseDto {Id = x.Id, Name = x.Name});
        }

        [HttpGet("{id:long}")]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.GetBy))]
        public async Task<ActionResult<BannedPhraseDto>> GetById(long id, CancellationToken ct = default)
        {
            var bannedPhrase = await _bannedPhraseRepository.GetByIdAsync(id);

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
                var entity = await _bannedPhraseRepository.AddAsync(new BannedPhrase {Name = request.Name});
                return entity is null ? Conflict() : CreatedAtAction(nameof(Create), entity.Id);
            }
        }

        [HttpDelete("{id:long}")]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Delete))]
        public async Task<ActionResult> Delete(long id, CancellationToken ct = default)
        {
            var bannedPhrase = await _bannedPhraseRepository.GetByIdAsync(id);
            if (bannedPhrase == null) return NotFound();
            
            await _bannedPhraseRepository.DeleteAsync(bannedPhrase);
            return Ok();
        }
    }
}