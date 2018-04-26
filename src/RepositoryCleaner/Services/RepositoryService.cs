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
    using Orc.FileSystem;

    internal class RepositoryService : IRepositoryService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ICleanerService _cleanerService;
        private readonly IDirectoryService _directoryService;

        public RepositoryService(ICleanerService cleanerService, IDirectoryService directoryService)
        {
            Argument.IsNotNull(() => cleanerService);
            Argument.IsNotNull(() => directoryService);

            _cleanerService = cleanerService;
            _directoryService = directoryService;
        }

        [Time]
        public virtual IEnumerable<Repository> FindRepositories(string repositoriesRoot)
        {
            Argument.IsNotNullOrWhitespace(() => repositoriesRoot);

            Log.Info("Searching for repositories in root '{0}'", repositoriesRoot);

            if (!_directoryService.Exists(repositoriesRoot))
            {
                Log.Warning("Directory '{0}' does not exist, cannot find any repositories", repositoriesRoot);
                return Enumerable.Empty<Repository>();
            }

            var cleanableRepositories = new List<Repository>();

            foreach (var directory in _directoryService.GetDirectories(repositoriesRoot, "*", SearchOption.AllDirectories))
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
            if (!_directoryService.Exists(gitDirectory))
            {
                Log.Debug("Directory '{0}' is not a repository because it does not contain a .git directory in the root", directory);
                return false;
            }

            var srcDirectory = Path.Combine(directory, "src");
            if (!_directoryService.Exists(srcDirectory))
            {
                Log.Debug("Directory '{0}' is not a repository because it does not contain a src directory in the root", directory);
                return false;
            }

            if (!_directoryService.GetFiles(srcDirectory, "*.sln").Any())
            {
                Log.Debug("Directory '{0}' is not a repository because it does not contain a .sln file in the src directory", directory);
                return false;
            }

            var libDirectory = Path.Combine(directory, "lib");
            if (!_directoryService.Exists(libDirectory))
            {
                Log.Debug("Directory '{0}' is not a repository because it does not contain a lib directory in the root", directory);
                return false;
            }

            Log.Debug("Directory '{0}' is considered a repository", directory);

            return true;
        }
    }
}