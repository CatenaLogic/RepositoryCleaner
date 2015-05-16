// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICleanerServiceExtensions.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Data;
    using Catel.Logging;
    using Catel.Threading;
    using Models;

    internal static class ICleanerServiceExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static IEnumerable<Repository> GetCleanableRepositories(this ICleanerService cleanerService, params Repository[] repositories)
        {
            Argument.IsNotNull(() => cleanerService);

            var cleanableRepositories = new List<Repository>();

            foreach (var repository in repositories)
            {
                if (cleanerService.CanClean(repository))
                {
                    cleanableRepositories.Add(repository);
                }
            }

            return cleanableRepositories;
        }

        public static async Task<IEnumerable<Repository>> GetCleanableRepositoriesAsync(this ICleanerService cleanerService, params Repository[] repositories)
        {
            Argument.IsNotNull(() => cleanerService);

            return await Task.Factory.StartNew(() => GetCleanableRepositories(cleanerService, repositories));
        }

        public static async Task<bool> CanCleanAsync(this ICleanerService cleanerService, Repository repository)
        {
            Argument.IsNotNull(() => cleanerService);

            return await Task.Factory.StartNew(() => cleanerService.CanClean(repository));
        }

        public static async Task CleanAsync(this ICleanerService cleanerService, params Repository[] repositories)
        {
            Argument.IsNotNull(() => cleanerService);

            var cleanedUpRepositories = new List<Repository>();

            var repositoriesToCleanUp = (from repository in repositories
                                         where repository.IsIncluded
                                         select repository).ToList();

            Log.Info("Cleaning up '{0}' repositories", repositoriesToCleanUp.Count);

            foreach (var repository in repositoriesToCleanUp)
            {
                // Note: we can also do them all async (don't await), but the disk is probably the bottleneck anyway
                await Task.Factory.StartNew(() => cleanerService.Clean(repository));

                cleanedUpRepositories.Add(repository);
            }

            Log.Info("Cleaned up '{0}' repositories", cleanedUpRepositories.Count);
        }
    }
}