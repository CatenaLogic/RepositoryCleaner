namespace RepositoryCleaner.Services
{
    using System;
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
            ArgumentNullException.ThrowIfNull(cleanerService);
            ArgumentNullException.ThrowIfNull(directoryService);

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

            foreach (var directory in _directoryService.GetDirectories(repositoriesRoot))
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
            if (_directoryService.Exists(gitDirectory))
            {
                Log.Debug("Directory '{0}' is a repository because it contains an .git directory in the root", directory);
                return true;
            }

            var srcDirectory = Path.Combine(directory, "src");
            if (_directoryService.Exists(srcDirectory))
            {
                Log.Debug("Directory '{0}' is a repository because it contains an src directory in the root", directory);
                return true;
            }

            if (_directoryService.GetFiles(directory, "*.sln").Any())
            {
                Log.Debug("Directory '{0}' is a repository because it contains a .sln file in the root", directory);
                return true;
            }

            Log.Debug("Directory '{0}' is not considered a repository", directory);

            return false;
        }
    }
}
