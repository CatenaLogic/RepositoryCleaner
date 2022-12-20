namespace RepositoryCleaner.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel.Logging;
    using Models;

    internal static class ICleanerServiceExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static async Task CleanAsync(this ICleanerService cleanerService, IEnumerable<Repository> repositories, bool isDryRun, Action completedCallback = null)
        {
            ArgumentNullException.ThrowIfNull(cleanerService);

            var cleanedUpRepositories = new List<Repository>();

            var repositoriesToCleanUp = (from repository in repositories
                                         where repository.IsIncluded
                                         select repository).ToList();

            Log.Info("Cleaning up '{0}' repositories", repositoriesToCleanUp.Count);

            foreach (var repository in repositoriesToCleanUp)
            {
                var cleanContext = new CleanContext(repository)
                {
                    IsDryRun = isDryRun
                };

                // Note: we can also do them all async (don't await), but the disk is probably the bottleneck anyway
                await cleanerService.CleanAsync(cleanContext);

                cleanedUpRepositories.Add(repository);

                if (completedCallback is not null)
                {
                    completedCallback();
                }
            }

            Log.Info("Cleaned up '{0}' repositories", cleanedUpRepositories.Count);
        }
    }
}
