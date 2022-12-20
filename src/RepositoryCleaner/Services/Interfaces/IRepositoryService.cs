namespace RepositoryCleaner.Services
{
    using System.Collections.Generic;
    using Models;

    internal interface IRepositoryService
    {
        IEnumerable<Repository> FindRepositories(string repositoriesRoot);
        bool IsRepository(string directory);
    }
}
