using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProfanityChecker.Domain;

namespace ProfanityChecker.Infrastructure.Configuration
{
    internal class BannedPhraseConfiguration : IEntityTypeConfiguration<BannedPhrase>
    {
        public void Configure(EntityTypeBuilder<BannedPhrase> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        }
    }
}