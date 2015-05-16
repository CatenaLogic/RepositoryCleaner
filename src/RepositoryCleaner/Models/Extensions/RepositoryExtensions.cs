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

    public static class RepositoryExtensions
    {
        public static async Task<long> CalculateCleanableSpaceAsync(this Repository repository)
        {
            Argument.IsNotNull(() => repository);

            return await Task.Factory.StartNew(() => CalculateCleanableSpace(repository));
        }

        public static long CalculateCleanableSpace(this Repository repository)
        {
            Argument.IsNotNull(() => repository);

            return repository.Cleaners.Sum(x => x.CalculateCleanableSpace(repository));
        }
    }
}