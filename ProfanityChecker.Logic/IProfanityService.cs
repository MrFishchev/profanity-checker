using System.Threading;
using System.Threading.Tasks;
using ProfanityChecker.Domain;

namespace ProfanityChecker.Logic
{
    public interface IProfanityService
    {
        Task<ProfanityScanResult> ScanAsync(string data, CancellationToken ct = default);
    }
}