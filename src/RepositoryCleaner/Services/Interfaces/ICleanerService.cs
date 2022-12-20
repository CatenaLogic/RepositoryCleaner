namespace RepositoryCleaner.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Cleaners;
    using Models;

    internal interface ICleanerService
    {
        event EventHandler<RepositoryEventArgs> RepositoryCleaning;
        event EventHandler<RepositoryEventArgs> RepositoryCleaned;

        #region Methods
        IEnumerable<ICleaner> GetAvailableCleaners();

        Task CleanAsync(CleanContext context);
        #endregion

        Task<bool> CanCleanAsync(Repository repository);
    }
}
