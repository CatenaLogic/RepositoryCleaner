// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICleaner.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.Cleaners
{
    using System.Threading.Tasks;
    using Models;

    public interface ICleaner
    {
        #region Properties
        string Name { get; }
        string Description { get; }
        #endregion

        #region Methods
        bool CanClean(Repository repository);

        long CalculateCleanableSpace(Repository repository);

        void Clean(Repository repository, bool isFakeClean);
        #endregion
    }
}