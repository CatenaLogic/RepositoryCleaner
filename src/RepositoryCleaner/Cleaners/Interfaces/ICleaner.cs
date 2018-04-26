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
        bool CanClean(CleanContext context);

        long CalculateCleanableSpace(CleanContext context);

        void Clean(CleanContext context);
        #endregion
    }
}