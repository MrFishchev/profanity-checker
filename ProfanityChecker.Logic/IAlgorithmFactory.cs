using System.Collections.Generic;
using System.Threading;

namespace ProfanityChecker.Logic
{
    public interface IAlgorithmFactory
    {
        ISearchingAlgorithm CreateAlgorithm<T>(IEnumerable<string> dictionary)
            where T : class, ISearchingAlgorithm;
    }
}