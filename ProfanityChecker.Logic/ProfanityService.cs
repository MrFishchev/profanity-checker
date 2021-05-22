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
        private readonly IAlgorithmFactory _algorithmFactory;
        private readonly ILogger<ProfanityService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public ProfanityService(IAlgorithmFactory algorithmFactory, IUnitOfWork unitOfWork,
            ILogger<ProfanityService> logger)
        {
            _algorithmFactory = algorithmFactory;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ProfanityScanResult> ScanAsync(string data, CancellationToken ct)
        {
            var dictionary = await _unitOfWork.BannedPhrases.GetAllAsync();
            var searchingAlgorithm = _algorithmFactory.CreateAlgorithm<AhoCorasickAlgorithm>(dictionary.Select(x => x.Name));
            var search = searchingAlgorithm.FindAll(data, ct).ToList();
            
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