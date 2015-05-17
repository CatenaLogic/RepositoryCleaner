// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CleanerBase.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.Cleaners
{
    using System;
    using System.IO;
    using System.Linq;
    using Catel;
    using Catel.Logging;
    using Catel.Reflection;
    using Models;

    public abstract class CleanerBase : ICleaner
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        protected CleanerBase()
        {
            CleanerAttribute cleanerAttribute = null;
            if (AttributeHelper.TryGetAttribute(GetType(), out cleanerAttribute))
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

        public bool CanClean(Repository repository)
        {
            Argument.IsNotNull(() => repository);

            Log.Debug("Checking if cleaner '{0}' can clean repository '{1}'", GetType(), repository);

            var canClean = CanCleanRepository(repository);

            Log.Debug("Cleaner '{0}' can clean repository '{1}': {2}", GetType(), repository, canClean);

            return canClean;
        }

        public long CalculateCleanableSpace(Repository repository)
        {
            Argument.IsNotNull(() => repository);

            if (!CanClean(repository))
            {
                return 0L;
            }

            Log.Debug("Calculating cleanable space using cleaner '{0}' and repository '{1}'", GetType(), repository);

            var cleanableSpace = CalculateCleanableSpaceForRepository(repository);

            Log.Debug("Calculated cleanable space using cleaner '{0}' and repository '{1}': {2}", GetType(), repository, cleanableSpace);

            return cleanableSpace;
        }

        public void Clean(Repository repository, bool isFakeClean)
        {
            Argument.IsNotNull(() => repository);

            if (!CanClean(repository))
            {
                return;
            }

            Log.Info("Cleaning up repository '{0}' using cleaner '{1}'", repository, GetType());

            CleanRepository(repository, isFakeClean);

            Log.Info("Cleaned up repository '{0}' using cleaner '{1}'", repository, GetType());
        }

        protected string GetRelativePath(Repository repository, string path)
        {
            Argument.IsNotNull(() => repository);

            return Path.Combine(repository.Directory, path);
        }

        protected void DeleteDirectory(string directory, bool isFakeClean)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            Log.Debug("Deleting directory '{0}'", directory);

            if (!isFakeClean)
            {
                try
                {
                    Directory.Delete(directory, true);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to delete directory '{0}'", directory);
                }
            }
        }

        protected long GetDirectorySize(string directory)
        {
            var size = 0L;

            try
            {
                if (Directory.Exists(directory))
                {
                    size += (from fileName in Directory.GetFiles(directory, "*", SearchOption.AllDirectories)
                             select new FileInfo(fileName)).Sum(x => x.Length);
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to calculate the cleanable space for directory '{0}'", directory);
            }

            return size;
        }

        protected abstract bool CanCleanRepository(Repository repository);

        protected abstract long CalculateCleanableSpaceForRepository(Repository repository);

        protected abstract void CleanRepository(Repository repository, bool isFakeClean);
    }
}