// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICleanerExtensions.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.Cleaners
{
    using System.Threading.Tasks;
    using Catel;
    using Models;

    internal static class ICleanerExtensions
    {
        public static async Task<long> CalculateCleanableSpaceAsync(this ICleaner cleaner, Repository repository)
        {
            Argument.IsNotNull(() => cleaner);
            Argument.IsNotNull(() => repository);

            return await Task.Factory.StartNew(() => cleaner.CalculateCleanableSpace(repository));
        }

        public static async Task CleanAsync(this ICleaner cleaner, Repository repository, bool isFakeClean)
        {
            Argument.IsNotNull(() => cleaner);
            Argument.IsNotNull(() => repository);

            await Task.Factory.StartNew(() => cleaner.Clean(repository, isFakeClean));
        }
    }
}