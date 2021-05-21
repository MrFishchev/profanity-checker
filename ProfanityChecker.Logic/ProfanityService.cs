using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProfanityChecker.Domain;
using ProfanityChecker.Infrastructure;

namespace ProfanityChecker.Logic
{
    public class ProfanityService : IProfanityService
    {
        private readonly ISearchingAlgorithm _searchingAlgorithm;
        private readonly IBannedPhraseRepository _bannedPhraseRepository;
        private readonly ILogger<ProfanityService> _logger;

        public ProfanityService(ISearchingAlgorithm searchingAlgorithm, IBannedPhraseRepository bannedPhraseRepository,
            ILogger<ProfanityService> logger)
        {
            // TODO: Resolve different algorithms
            _searchingAlgorithm = searchingAlgorithm;
            _bannedPhraseRepository = bannedPhraseRepository;
            _logger = logger;
        }

        public async Task<ProfanityScanResult> ScanAsync(string data, CancellationToken ct)
        {
            var dictionary = await _bannedPhraseRepository.GetAllAsync();
            var search = _searchingAlgorithm.FindAll(data, ct);

            if (!search.Any())
            {
                _logger.LogInformation("Profanity not found");
                return ProfanityScanResult.NoProfanity;
            }
            
            _logger.LogInformation($"{search.Count()} profanity are found");
            return ProfanityScanResult.WithProfanity(search.ToList());
        }
    }
}