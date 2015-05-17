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
        public virtual IEnumerable<Repository> FindRepositories(string repositoriesRoot)
        {
            Argument.IsNotNullOrWhitespace(() => repositoriesRoot);

            Log.Info("Searching for repositories in root '{0}'", repositoriesRoot);

            if (!Directory.Exists(repositoriesRoot))
            {
                Log.Warning("Directory '{0}' does not exist, cannot find any repositories", repositoriesRoot);
                return Enumerable.Empty<Repository>();
            }

            var cleanableRepositories = new List<Repository>();

            foreach (var directory in Directory.GetDirectories(repositoriesRoot))
            {
                if (IsRepository(directory))
                {
                    var repository = new Repository(directory, _cleanerService.GetAvailableCleaners());
                    cleanableRepositories.Add(repository);
                }
            }

            Log.Info("Found {0} repositories in root '{1}'", cleanableRepositories.Count, repositoriesRoot);

            return cleanableRepositories;
        }

        public virtual bool IsRepository(string directory)
        {
            // We have several rules out of the box to determine if a directory is a repository

            Log.Debug("Checking if a '{0}' is a repository", directory);

            var gitDirectory = Path.Combine(directory, ".git");
            if (Directory.Exists(gitDirectory))
            {
                Log.Debug("Directory '{0}' is a repository because it contains an .git directory in the root", directory);
                return true;
            }

            var srcDirectory = Path.Combine(directory, "src");
            if (Directory.Exists(srcDirectory))
            {
                Log.Debug("Directory '{0}' is a repository because it contains an src directory in the root", directory);
                return true;
            }

            if (Directory.GetFiles(directory, "*.sln").Any())
            {
                Log.Debug("Directory '{0}' is a repository because it contains a .sln file in the root", directory);
                return true;
            }

            Log.Debug("Directory '{0}' is not considered a repository", directory);

            return false;
        }
    }
}