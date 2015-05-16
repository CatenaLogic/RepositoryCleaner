// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CleanerService.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Data;
    using Catel.Logging;
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
        public bool CanClean(Repository repository)
        {
            var canClean = false;

            Log.Debug("Checking if repository '{0}' can be cleaned", repository);
            Log.Indent();

            var cleaners = GetAvailableCleaners();
            foreach (var cleaner in cleaners)
            {
                Log.Debug("Checking if repository '{0}' can be cleaned by cleaner '{1}'", repository, cleaner);

                if (cleaner.CanClean(repository))
                {
                    Log.Debug("Repository '{0}' can be cleaned by cleaner '{1}'", repository, cleaner);

                    canClean = true;
                    break;
                }
            }

            Log.Unindent();
            Log.Debug("Checked if repository '{0}' can be cleaned, result = {1}", repository, canClean);

            return canClean;
        }

        [Time]
        public void Clean(Repository repository)
        {
            RepositoryCleaning.SafeInvoke(this, new RepositoryEventArgs(repository));

            Log.Info("Cleaning repository '{0}'", repository);
            Log.Indent();

            var cleaners = GetAvailableCleaners();
            foreach (var cleaner in cleaners)
            {
                if (cleaner.CanClean(repository))
                {
                    Log.Debug("Cleaning repository '{0}' using cleaner '{1}'", repository, cleaner);

                    cleaner.Clean(repository);

                    Log.Debug("Cleaned repository '{0}' using cleaner '{1}'", repository, cleaner);
                }
            }

            Log.Unindent();
            Log.Info("Cleaned repository '{0}'", repository);

            RepositoryCleaned.SafeInvoke(this, new RepositoryEventArgs(repository));
        }
    }
}