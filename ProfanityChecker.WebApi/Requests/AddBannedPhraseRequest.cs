namespace ProfanityChecker.WebApi.Requests
{
    public sealed record AddBannedPhraseRequest
    {
        public string Name { get; init; }    
    }
}