// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryExtensions.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;

    public static class RepositoryExtensions
    {
        private const int MaxRunningthreads = 5;

        public static async Task CalculateCleanableSpaceAsyncAndMultithreaded(this List<Repository> repositories, Action completedCallback = null)
        {
            var itemsPerBatch = repositories.Count / MaxRunningthreads;

            await Task.Factory.StartNew(() => ParallelHelper.ExecuteInParallel(repositories,
                repository =>
                {
                    repository.CalculateCleanableSpace();

                    if (completedCallback != null)
                    {
                        completedCallback();
                    }
                }, itemsPerBatch));
        }

        public static async Task<long> CalculateCleanableSpaceAsync(this Repository repository)
        {
            Argument.IsNotNull(() => repository);

            return await Task.Factory.StartNew(() => CalculateCleanableSpace(repository));
        }

        public static long CalculateCleanableSpace(this Repository repository)
        {
            Argument.IsNotNull(() => repository);

            if (!repository.CleanableSize.HasValue)
            {
                repository.CleanableSize = repository.Cleaners.Sum(x => x.CalculateCleanableSpace(repository));
            }

            return repository.CleanableSize ?? 0L;
        }
    }
}