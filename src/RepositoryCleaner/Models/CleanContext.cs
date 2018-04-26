// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CleanContext.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2018 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.Models
{
    using Catel;

    public class CleanContext
    {
        public CleanContext(Repository repository)
        {
            Argument.IsNotNull(() => repository);

            Repository = repository;
        }

        public Repository Repository { get; private set; }

        public bool IsDryRun { get; set; }
    }
}