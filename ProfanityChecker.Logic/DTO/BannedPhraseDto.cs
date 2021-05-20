namespace ProfanityChecker.Logic.DTO
{
    public sealed record BannedPhraseDto
    {
        public long Id { get; init; }

        public string Name { get; init; }
    }
}