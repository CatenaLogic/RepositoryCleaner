// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICleanerService.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.Services
{
    using System;
    using System.Collections.Generic;
    using Cleaners;
    using Models;

    internal interface ICleanerService
    {
        event EventHandler<RepositoryEventArgs> RepositoryCleaning;
        event EventHandler<RepositoryEventArgs> RepositoryCleaned;

        #region Methods
        IEnumerable<ICleaner> GetAvailableCleaners();

        void Clean(Repository repository, bool isFakeClean);
        #endregion

        bool CanClean(Repository repository);
    }
}