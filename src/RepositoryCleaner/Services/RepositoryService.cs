// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryService.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Catel;
    using Catel.Logging;
    using MethodTimer;
    using Models;

    internal class RepositoryService : IRepositoryService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ICleanerService _cleanerService;

        public RepositoryService(ICleanerService cleanerService)
        {
            Argument.IsNotNull(() => cleanerService);

            _cleanerService = cleanerService;
        }

        [Time]
        public IEnumerable<Repository> FindRepositories(string repositoriesRoot)
        {
            Argument.IsNotNullOrWhitespace(() => repositoriesRoot);

            Log.Info("Searching for repositories in root '{0}'", repositoriesRoot);

            if (!Directory.Exists(repositoriesRoot))
            {
                Log.Warning("Directory '{0}' does not exist, cannot find any repositories", repositoriesRoot);
                return Enumerable.Empty<Repository>();
            }

            var repositories = (from directory in Directory.GetDirectories(repositoriesRoot)
                                select new Repository(directory, _cleanerService.GetAvailableCleaners()));

            var cleanableRepositories = (from repository in repositories
                                         where _cleanerService.CanClean(repository)
                                         select repository).ToList();

            Log.Info("Found {0} repositories in root '{1}'", cleanableRepositories.Count, repositoriesRoot);

            return cleanableRepositories;
        } 
    }
}