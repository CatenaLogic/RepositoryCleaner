// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICleanerServiceExtensions.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.Services
{
    using System;
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

        public static async Task CleanAsync(this ICleanerService cleanerService, IEnumerable<Repository> repositories, bool isFakeClean, Action completedCallback = null)
        {
            Argument.IsNotNull(nameof(cleanerService), cleanerService);

            var cleanedUpRepositories = new List<Repository>();

            var repositoriesToCleanUp = (from repository in repositories
                                         where repository.IsIncluded
                                         select repository).ToList();

            Log.Info("Cleaning up '{0}' repositories", repositoriesToCleanUp.Count);

            foreach (var repository in repositoriesToCleanUp)
            {
                // Note: we can also do them all async (don't await), but the disk is probably the bottleneck anyway
                await cleanerService.CleanAsync(repository, isFakeClean);

                cleanedUpRepositories.Add(repository);

                if (completedCallback != null)
                {
                    completedCallback();
                }
            }

            Log.Info("Cleaned up '{0}' repositories", cleanedUpRepositories.Count);
        }
    }
}