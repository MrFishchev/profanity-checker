using System.Collections.Generic;

namespace ProfanityChecker.Logic
{
    public interface IAlgorithmFactory
    {
        /// <summary>
        /// Creates instance of an algorithm
        /// </summary>
        /// <param name="dictionary">List of patterns for searching</param>
        /// <typeparam name="T">Type of an algorithm</typeparam>
        /// <returns>Instance of an algorithm</returns>
        ISearchingAlgorithm CreateAlgorithm<T>(IEnumerable<string> dictionary)
            where T : class, ISearchingAlgorithm;
    }
}