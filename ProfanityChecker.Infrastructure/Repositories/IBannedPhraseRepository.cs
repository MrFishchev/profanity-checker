using System.Threading;
using System.Threading.Tasks;
using ProfanityChecker.Domain;

// ReSharper disable once CheckNamespace
namespace ProfanityChecker.Infrastructure
{
    public interface IBannedPhraseRepository : IRepository<BannedPhrase>
    {
        Task<BannedPhrase> GetByIdAsync(long id, CancellationToken ct = default);
    }
}