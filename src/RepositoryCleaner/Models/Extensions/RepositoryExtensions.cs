// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryExtensions.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.Models
{
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Logging;
    using Catel.Threading;
    using MethodTimer;

    public static class RepositoryExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        [Time]
        public static async Task<long> CalculateCleanableSpaceAsync(this Repository repository)
        {
            Argument.IsNotNull(() => repository);

            Log.Debug($"Calculating cleanable space for {repository}");

            if (!repository.CleanableSize.HasValue)
            {
                // Actual caclulation goes on a separate thread
                var cleanableSize = await TaskShim.Run(() => repository.Cleaners.Sum(x => x.CalculateCleanableSpace(repository)));
                repository.CleanableSize = cleanableSize;
            }

            return repository.CleanableSize ?? 0L;
        }
    }
}