using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProfanityChecker.Domain;

namespace ProfanityChecker.Infrastructure
{
    public class BannedPhraseRepository : Repository<BannedPhrase>, IBannedPhraseRepository
    {
        public BannedPhraseRepository(DataContext dataContext) : base(dataContext)
        {
        }

        public Task<BannedPhrase> GetByIdAsync(long id, CancellationToken ct)
        {
            return DataContext.Set<BannedPhrase>().FirstOrDefaultAsync(x => x.Id == id, ct);
        }
    }
}