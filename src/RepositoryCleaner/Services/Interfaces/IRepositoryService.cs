// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryService.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


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