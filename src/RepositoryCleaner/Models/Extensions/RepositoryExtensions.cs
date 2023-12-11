namespace RepositoryCleaner.Models
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel.Logging;
    using MethodTimer;

    public static class RepositoryExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        [Time]
        public static async Task<ulong> CalculateCleanableSpaceAsync(this Repository repository)
        {
            ArgumentNullException.ThrowIfNull(repository);

            Log.Debug($"Calculating cleanable space for {repository}");

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
