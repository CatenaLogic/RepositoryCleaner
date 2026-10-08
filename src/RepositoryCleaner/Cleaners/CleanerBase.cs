namespace RepositoryCleaner.Cleaners
{
    using System;
    using System.IO;
    using System.Reflection;
    using Catel.Logging;
    using MethodTimer;
    using Microsoft.Extensions.Logging;
    using Models;
    using Orc.FileSystem;

    public abstract class CleanerBase : ICleaner
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(CleanerBase));

        protected readonly IDirectoryService _directoryService;

        protected CleanerBase(IDirectoryService directoryService)
        {
            ArgumentNullException.ThrowIfNull(directoryService);

            _directoryService = directoryService;

            var cleanerAttribute = GetType().GetCustomAttribute<CleanerAttribute>();
            if (cleanerAttribute is not null)
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

            Logger.LogDebug("Checking if cleaner {CleanerType} can clean repository {Repository}", GetType(), context.Repository);

            var canClean = CanCleanRepository(context);

            Logger.LogDebug("Cleaner {CleanerType} can clean repository {Repository}: {CanClean}", GetType(), context.Repository, canClean);

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

            Logger.LogDebug("Calculating cleanable space using cleaner {CleanerType} and repository {Repository}", GetType(), context.Repository);

            var cleanableSpace = CalculateCleanableSpaceForRepository(context);

            Logger.LogDebug("Calculated cleanable space using cleaner {CleanerType} and repository {Repository}: {CleanableSpace}", GetType(), context.Repository, cleanableSpace);

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

            Logger.LogInformation("Cleaning up repository {Repository} using cleaner {CleanerType}", context.Repository, GetType());

            CleanRepository(context);

            Logger.LogInformation("Cleaned up repository {Repository} using cleaner {CleanerType}", context.Repository, GetType());
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

            Logger.LogDebug("Deleting directory {Directory}", directory);

            if (!context.IsDryRun)
            {
                try
                {
                    _directoryService.Delete(directory);
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Failed to delete directory {Directory}", directory);
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
