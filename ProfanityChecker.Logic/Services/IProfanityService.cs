using System.Threading;
using System.Threading.Tasks;
using ProfanityChecker.Domain;

// ReSharper disable once CheckNamespace
namespace ProfanityChecker.Logic
{
    public interface IProfanityService
    {
        Task<ProfanityScanResult> ScanAsync(string data, CancellationToken ct = default);
    }
}