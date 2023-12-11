namespace RepositoryCleaner.Cleaners
{
    using System;
    using System.IO;
    using Catel.Logging;
    using Catel.Reflection;
    using MethodTimer;
    using Models;
    using Orc.FileSystem;

    public abstract class CleanerBase : ICleaner
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        protected readonly IDirectoryService _directoryService;

        protected CleanerBase(IDirectoryService directoryService)
        {
            ArgumentNullException.ThrowIfNull(directoryService);

            _directoryService = directoryService;

            if (GetType().TryGetAttribute(out CleanerAttribute cleanerAttribute))
            {
                Name = cleanerAttribute.Name;
                Description = cleanerAttribute.Description;
            }
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public bool CanClean(CleanContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            Log.Debug("Checking if cleaner '{0}' can clean repository '{1}'", GetType(), context.Repository);

            var canClean = CanCleanRepository(context);

            Log.Debug("Cleaner '{0}' can clean repository '{1}': {2}", GetType(), context.Repository, canClean);

            return canClean;
        }

        [Time]
        public ulong CalculateCleanableSpace(CleanContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (!CanClean(context))
            {
                return 0L;
            }

            Log.Debug("Calculating cleanable space using cleaner '{0}' and repository '{1}'", GetType(), context.Repository);

            var cleanableSpace = CalculateCleanableSpaceForRepository(context);

            Log.Debug("Calculated cleanable space using cleaner '{0}' and repository '{1}': {2}", GetType(), context.Repository, cleanableSpace);

            return cleanableSpace;
        }

        [Time]
        public void Clean(CleanContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (!CanClean(context))
            {
                return;
            }

            Log.Info("Cleaning up repository '{0}' using cleaner '{1}'", context.Repository, GetType());

            CleanRepository(context);

            Log.Info("Cleaned up repository '{0}' using cleaner '{1}'", context.Repository, GetType());
        }

        protected string GetRelativePath(Repository repository, string path)
        {
            ArgumentNullException.ThrowIfNull(repository);

            return Path.Combine(repository.Directory, path);
        }

        protected void DeleteDirectory(string directory, CleanContext context)
        {
            if (!_directoryService.Exists(directory))
            {
                return;
            }

            Log.Debug("Deleting directory '{0}'", directory);

            if (!context.IsDryRun)
            {
                try
                {
                    _directoryService.Delete(directory);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to delete directory '{0}'", directory);
                }
            }
        }

        protected ulong GetDirectorySize(string directory)
        {
            return _directoryService.GetSize(directory);
        }

        protected abstract bool CanCleanRepository(CleanContext context);

        protected abstract ulong CalculateCleanableSpaceForRepository(CleanContext context);

        protected abstract void CleanRepository(CleanContext context);
    }
}
