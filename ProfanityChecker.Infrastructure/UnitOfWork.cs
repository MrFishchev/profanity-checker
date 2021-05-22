using System;
using System.Threading.Tasks;

namespace ProfanityChecker.Infrastructure
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        public IBannedPhraseRepository BannedPhrases { get; }

        public UnitOfWork(IBannedPhraseRepository bannedPhrases, DataContext context)
        {
            BannedPhrases = bannedPhrases;
            _context = context;
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
                _context.Dispose();
        }
    }
}