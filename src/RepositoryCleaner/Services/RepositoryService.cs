namespace RepositoryCleaner.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Catel;
    using MethodTimer;
    using Microsoft.Extensions.Logging;
    using Models;
    using Orc.FileSystem;

    internal class RepositoryService : IRepositoryService
    {
        private readonly ILogger<RepositoryService> _logger;

        private readonly ICleanerService _cleanerService;
        private readonly IDirectoryService _directoryService;

        public RepositoryService(ICleanerService cleanerService, IDirectoryService directoryService, ILogger<RepositoryService> logger)
        {
            ArgumentNullException.ThrowIfNull(cleanerService);
            ArgumentNullException.ThrowIfNull(directoryService);
            ArgumentNullException.ThrowIfNull(logger);

            _cleanerService = cleanerService;
            _directoryService = directoryService;
            _logger = logger;
        }

        [Time]
        public virtual IEnumerable<Repository> FindRepositories(string repositoriesRoot)
        {
            Argument.IsNotNullOrWhitespace(() => repositoriesRoot);

            _logger.LogInformation("Searching for repositories in root {RepositoriesRoot}", repositoriesRoot);

            if (!_directoryService.Exists(repositoriesRoot))
            {
                _logger.LogWarning("Directory {RepositoriesRoot} does not exist, cannot find any repositories", repositoriesRoot);
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

            _logger.LogInformation("Found {RepositoryCount} repositories in root {RepositoriesRoot}", cleanableRepositories.Count, repositoriesRoot);

            return cleanableRepositories;
        }

        public virtual bool IsRepository(string directory)
        {
            // We have several rules out of the box to determine if a directory is a repository

            _logger.LogDebug("Checking if {Directory} is a repository", directory);

            var gitDirectory = Path.Combine(directory, ".git");
            if (_directoryService.Exists(gitDirectory))
            {
                _logger.LogDebug("Directory {Directory} is a repository because it contains an .git directory in the root", directory);
                return true;
            }

            var srcDirectory = Path.Combine(directory, "src");
            if (_directoryService.Exists(srcDirectory))
            {
                _logger.LogDebug("Directory {Directory} is a repository because it contains an src directory in the root", directory);
                return true;
            }

            if (_directoryService.GetFiles(directory, "*.sln").Any())
            {
                _logger.LogDebug("Directory {Directory} is a repository because it contains a .sln file in the root", directory);
                return true;
            }

            _logger.LogDebug("Directory {Directory} is not considered a repository", directory);

            return false;
        }
    }
}
