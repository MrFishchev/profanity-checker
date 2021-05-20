using Microsoft.EntityFrameworkCore;
using ProfanityChecker.Domain;
using ProfanityChecker.Infrastructure.Configuration;

namespace ProfanityChecker.Infrastructure
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        
        public DbSet<BannedPhrase> BannedPhrases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BannedPhraseConfiguration());
        }
    }
}