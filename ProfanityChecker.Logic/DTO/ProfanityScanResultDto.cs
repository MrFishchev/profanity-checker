#nullable enable
using System.Collections.Generic;
using ProfanityChecker.Domain;

namespace ProfanityChecker.Logic.DTO
{
    public sealed record ProfanityScanResultDto(bool HasProfanity, IEnumerable<ProfanityItem>? ProfanityItems);
}