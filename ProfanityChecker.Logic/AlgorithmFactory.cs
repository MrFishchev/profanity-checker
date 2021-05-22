using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace ProfanityChecker.Logic
{
    public class AlgorithmFactory : IAlgorithmFactory
    {
        private readonly ILogger<AlgorithmFactory> _logger;

        public AlgorithmFactory(ILogger<AlgorithmFactory> logger)
        {
            _logger = logger;
        }
        
        public ISearchingAlgorithm CreateAlgorithm<T>(IEnumerable<string> dictionary)
            where T : class, ISearchingAlgorithm
        {
            return CreateInstance<T>(dictionary);
        }

        private ISearchingAlgorithm CreateInstance<T>(IEnumerable<string> dictionary)
            where T: class, ISearchingAlgorithm
        {
            try
            {
                var factoryMethod = typeof(T).GetMethod("Create");
                return factoryMethod?.Invoke(null, new[] {dictionary}) as ISearchingAlgorithm;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unable to activate instance of type {typeof(T)}");
                throw;
            }
        }
    }
}