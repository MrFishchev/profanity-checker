using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProfanityChecker.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IBannedPhraseRepository BannedPhrases { get; }
        
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}