namespace RepositoryCleaner.Models
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel.Logging;
    using MethodTimer;
    using Microsoft.Extensions.Logging;

    public static class RepositoryExtensions
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(RepositoryExtensions));

        [Time]
        public static async Task<ulong> CalculateCleanableSpaceAsync(this Repository repository)
        {
            ArgumentNullException.ThrowIfNull(repository);

            Logger.LogDebug("Calculating cleanable space for {Repository}", repository);

            if (!repository.CleanableSize.HasValue)
            {
                // Actual calculation goes on a separate thread
                var cleanableSize = await Task.Run(() => (ulong)repository.Cleaners.Sum(x => (long)x.CalculateCleanableSpace(new CleanContext(repository))));
                repository.CleanableSize = cleanableSize;
            }

            return repository.CleanableSize ?? 0L;
        }
    }
}
