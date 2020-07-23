// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetPackagesInLibFolderCleaner.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.Cleaners
{
    using Models;
    using Orc.FileSystem;

    [Cleaner("NuGetPackagesInLibFolderCleaner", Description = "Delete NuGet packages inside a /lib folder of a repository")]
    public class NuGetPackagesInLibFolderCleaner : NuGetPackagesCleanerBase
    {
        public NuGetPackagesInLibFolderCleaner(IDirectoryService directoryService) 
            : base(directoryService)
        {
        }

        protected override string GetPackagesDirectory(Repository repository)
        {
            // TODO: we could make it smarter by checking where the sln lives. Then use the packages in there or respect the nuget.config

            var libPath = GetRelativePath(repository, "lib");
            return libPath;
        }
    }
}