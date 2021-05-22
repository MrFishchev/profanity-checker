using System;
using System.Threading.Tasks;

namespace ProfanityChecker.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IBannedPhraseRepository BannedPhrases { get; }
        Task SaveChangesAsync();
    }
}