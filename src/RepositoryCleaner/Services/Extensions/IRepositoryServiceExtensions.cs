// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryServiceExtensions.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Catel;
    using Models;

    internal static class IRepositoryServiceExtensions
    {
        public static async Task<IEnumerable<Repository>> FindRepositoriesAsync(this IRepositoryService repositoryService, string repositoriesRoot)
        {
            Argument.IsNotNull(() => repositoryService);

            return await Task.Factory.StartNew(() => repositoryService.FindRepositories(repositoriesRoot));
        }
    }
}