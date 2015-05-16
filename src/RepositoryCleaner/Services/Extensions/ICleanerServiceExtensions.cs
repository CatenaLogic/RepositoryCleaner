// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICleanerServiceExtensions.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Data;
    using Models;

    internal static class ICleanerServiceExtensions
    {
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

            foreach (var repository in repositories)
            {
                await Task.Factory.StartNew(() => cleanerService.Clean(repository));
            }
        }
    }
}