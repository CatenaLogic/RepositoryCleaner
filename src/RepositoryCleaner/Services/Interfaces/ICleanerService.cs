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

        IReadOnlyList<ICleaner> GetAvailableCleaners();

        Task CleanAsync(CleanContext context);

        Task<bool> CanCleanAsync(Repository repository);
    }
}
