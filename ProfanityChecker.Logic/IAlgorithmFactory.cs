using System.Collections.Generic;

namespace ProfanityChecker.Logic
{
    public interface IAlgorithmFactory
    {
        ISearchingAlgorithm CreateAlgorithm<T>(IEnumerable<string> dictionary)
            where T : class, ISearchingAlgorithm;
    }
}