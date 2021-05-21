using System.Threading.Tasks;
using ProfanityChecker.Domain;

namespace ProfanityChecker.Infrastructure
{
    public interface IBannedPhraseRepository : IRepository<BannedPhrase>
    {
        Task<BannedPhrase> GetByIdAsync(long id);
    }
}