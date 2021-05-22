using System.Threading;
using System.Threading.Tasks;
using ProfanityChecker.Domain;

// ReSharper disable once CheckNamespace
namespace ProfanityChecker.Logic
{
    public interface IProfanityService
    {
        /// <summary>
        /// Scans text data for profanity
        /// </summary>
        /// <param name="data">Text data that will be scanned</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Profanity scan result</returns>
        Task<ProfanityScanResult> ScanAsync(string data, CancellationToken ct = default);
    }
}