using System;

namespace ProfanityChecker.Domain
{
    public sealed class BannedPhrase
    {
        public long Id { get; }

        public string Name { get; set; }

        public BannedPhrase() : this("")
        {
            
        }
        
        public BannedPhrase(string? name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(Name));
        }
        
        public BannedPhrase(long id, string? name)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(Name));
        }
    }
}