namespace RepositoryCleaner.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Cleaners;
    using MethodTimer;
    using Models;
    using System.Linq;

    internal class CleanerService : ICleanerService
    {
        private readonly ILogger<CleanerService> _logger;
        private readonly IReadOnlyList<ICleaner> _cleaners;

        public CleanerService(IServiceProvider serviceProvider, ILogger<CleanerService> logger,
            IEnumerable<ICleaner> cleaners)
        {
            _logger = logger;
            _cleaners = cleaners.ToArray();
        }

        public event EventHandler<RepositoryEventArgs> RepositoryCleaning;
        public event EventHandler<RepositoryEventArgs> RepositoryCleaned;

        public IReadOnlyList<ICleaner> GetAvailableCleaners()
        {
            return _cleaners;
        }

        [Time]
        public async Task<bool> CanCleanAsync(Repository repository)
        {
            var canClean = false;

            _logger.LogDebug("Checking if repository {Repository} can be cleaned", repository);

            var cleaners = GetAvailableCleaners();

            await Task.Run(() =>
            {
                foreach (var cleaner in cleaners)
                {
                    _logger.LogDebug("Checking if repository {Repository} can be cleaned by cleaner {Cleaner}", repository, cleaner);

                    if (cleaner.CanClean(new CleanContext(repository)))
                    {
                        _logger.LogDebug("Repository {Repository} can be cleaned by cleaner {Cleaner}", repository, cleaner);

                        canClean = true;
                        break;
                    }
                }
            });

            _logger.LogDebug("Checked if repository {Repository} can be cleaned, result = {CanClean}", repository, canClean);

            return canClean;
        }

        [Time]
        public async Task CleanAsync(CleanContext context)
        {
            RepositoryCleaning?.Invoke(this, new RepositoryEventArgs(context.Repository));

            _logger.LogInformation("Cleaning repository {Repository}", context.Repository);

            await Task.Run(() =>
            {
                var cleaners = GetAvailableCleaners();
                foreach (var cleaner in cleaners)
                {
                    if (cleaner.CanClean(context))
                    {
                        _logger.LogDebug("Cleaning repository {Repository} using cleaner {Cleaner}", context.Repository, cleaner);

                        cleaner.Clean(context);

                        _logger.LogDebug("Cleaned repository {Repository} using cleaner {Cleaner}", context.Repository, cleaner);
                    }
                }
            });

            _logger.LogInformation("Cleaned repository {Repository}", context.Repository);

            RepositoryCleaned?.Invoke(this, new RepositoryEventArgs(context.Repository));
        }
    }
}
