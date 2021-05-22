using System.Collections.Generic;
using System.Threading;
using ProfanityChecker.Domain;

namespace ProfanityChecker.Logic
{
    public interface ISearchingAlgorithm
    {
        IEnumerable<ProfanityItem> FindAll(string data, CancellationToken ct = default);

        bool ContainsAny(string data, CancellationToken ct = default);
    }
}