namespace RepositoryCleaner.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    internal static class IRepositoryServiceExtensions
    {
        public static async Task<IEnumerable<Repository>> FindRepositoriesAsync(this IRepositoryService repositoryService, string repositoriesRoot)
        {
            ArgumentNullException.ThrowIfNull(repositoryService);

            return await Task.Factory.StartNew(() => repositoryService.FindRepositories(repositoriesRoot));
        }
    }
}
