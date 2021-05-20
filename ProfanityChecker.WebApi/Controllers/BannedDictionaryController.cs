using System.Collections.Generic;
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
        public IAsyncEnumerable<BannedPhraseDto> GetAll(CancellationToken ct = default)
        {
            var bannedDictionary = _bannedPhraseRepository.GetAll();
            return GetAllAsync();

            async IAsyncEnumerable<BannedPhraseDto> GetAllAsync()
            {
                await foreach (var bannedPhrase in bannedDictionary.WithCancellation(ct))
                {
                    yield return new BannedPhraseDto {Id = bannedPhrase.Id, Name = bannedPhrase.Name};
                }
            }
        }

        [HttpPost]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Create))]
        public async Task<ActionResult<long>> Create(AddBannedPhraseRequest request, CancellationToken ct = default)
        {
            var entity = await _bannedPhraseRepository.AddAsync(new BannedPhrase {Name = request.Name});
            return entity is null ? Conflict() : CreatedAtAction(nameof(Create), entity.Id);
        }
    }
}