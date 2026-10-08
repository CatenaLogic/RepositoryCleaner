namespace RepositoryCleaner.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel.Logging;
    using Microsoft.Extensions.Logging;
    using Models;

    internal static class ICleanerServiceExtensions
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(ICleanerServiceExtensions));

        public static async Task CleanAsync(this ICleanerService cleanerService, IEnumerable<Repository> repositories, bool isDryRun, Action completedCallback = null)
        {
            ArgumentNullException.ThrowIfNull(cleanerService);

            var cleanedUpRepositories = new List<Repository>();

            var repositoriesToCleanUp = (from repository in repositories
                                         where repository.IsIncluded
                                         select repository).ToList();

            Logger.LogInformation("Cleaning up {RepositoryCount} repositories", repositoriesToCleanUp.Count);

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

            Logger.LogInformation("Cleaned up {RepositoryCount} repositories", cleanedUpRepositories.Count);
        }
    }
}
