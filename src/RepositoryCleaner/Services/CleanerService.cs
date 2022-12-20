namespace RepositoryCleaner.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Data;
    using Catel.Logging;
    using Catel.Threading;
    using Cleaners;
    using MethodTimer;
    using Models;

    internal class CleanerService : InterfaceFinderServiceBase<ICleaner>, ICleanerService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public event EventHandler<RepositoryEventArgs> RepositoryCleaning;
        public event EventHandler<RepositoryEventArgs> RepositoryCleaned;

        public IEnumerable<ICleaner> GetAvailableCleaners()
        {
            return GetAvailableItems();
        }

        [Time]
        public async Task<bool> CanCleanAsync(Repository repository)
        {
            var canClean = false;

            Log.Debug("Checking if repository '{0}' can be cleaned", repository);
            Log.Indent();

            var cleaners = GetAvailableCleaners();

            await TaskShim.Run(() =>
            {
                foreach (var cleaner in cleaners)
                {
                    Log.Debug("Checking if repository '{0}' can be cleaned by cleaner '{1}'", repository, cleaner);

                    if (cleaner.CanClean(new CleanContext(repository)))
                    {
                        Log.Debug("Repository '{0}' can be cleaned by cleaner '{1}'", repository, cleaner);

                        canClean = true;
                        break;
                    }
                }
            });

            Log.Unindent();
            Log.Debug("Checked if repository '{0}' can be cleaned, result = {1}", repository, canClean);

            return canClean;
        }

        [Time]
        public async Task CleanAsync(CleanContext context)
        {
            RepositoryCleaning?.Invoke(this, new RepositoryEventArgs(context.Repository));

            Log.Info("Cleaning repository '{0}'", context.Repository);
            Log.Indent();

            await TaskHelper.RunAndWaitAsync(() =>
            {
                var cleaners = GetAvailableCleaners();
                foreach (var cleaner in cleaners)
                {
                    if (cleaner.CanClean(context))
                    {
                        Log.Debug("Cleaning repository '{0}' using cleaner '{1}'", context.Repository, cleaner);

                        cleaner.Clean(context);

                        Log.Debug("Cleaned repository '{0}' using cleaner '{1}'", context.Repository, cleaner);
                    }
                }
            });

            Log.Unindent();
            Log.Info("Cleaned repository '{0}'", context.Repository);

            RepositoryCleaned?.Invoke(this, new RepositoryEventArgs(context.Repository));
        }
    }
}
