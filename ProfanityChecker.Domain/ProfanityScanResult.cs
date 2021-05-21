#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProfanityChecker.Domain
{
    public sealed class ProfanityScanResult
    {
        public bool HasProfanity => ProfanityItems?.Any() ?? false;
        public List<ProfanityItem>? ProfanityItems { get; }
        
        private ProfanityScanResult(List<ProfanityItem>? profanityItems)
        {
            ProfanityItems = profanityItems;
        }

        public static ProfanityScanResult WithProfanity(List<ProfanityItem> profanityItems)
        {
            if (!profanityItems.Any())
                throw new ArgumentException("A collection cannot be empty", nameof(profanityItems));

            return new ProfanityScanResult(profanityItems);
        }

        public static ProfanityScanResult NoProfanity => new ProfanityScanResult(null);
    }
}