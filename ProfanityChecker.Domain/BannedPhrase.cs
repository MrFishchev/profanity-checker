namespace ProfanityChecker.Domain
{
    public sealed class BannedPhrase
    {
        public long Id { get; init; }

        public string Name { get; set; }
    }
}