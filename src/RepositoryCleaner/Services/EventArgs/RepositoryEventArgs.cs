// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryEventArgs.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.Services
{
    using System;
    using Models;

    internal class RepositoryEventArgs : EventArgs
    {
        public RepositoryEventArgs(Repository repository)
        {
            Repository = repository;
        }

        public Repository Repository { get; set; }
    }
}