// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntermediateFilesCleaner.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.Cleaners
{
    using System.IO;
    using Catel.Logging;
    using Models;
    using Orc.FileSystem;

    [Cleaner("OutputDirectoryInRootCleaner", Description = "Delete output directory in the root (thus [repository]\\output")]
    public class OutputDirectoryInRootCleaner : CleanerBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public OutputDirectoryInRootCleaner(IDirectoryService directoryService) 
            : base(directoryService)
        {
        }

        protected override bool CanCleanRepository(CleanContext context)
        {
            var path = GetOutputDirectory(context.Repository);

            return _directoryService.Exists(path);
        }

        protected override ulong CalculateCleanableSpaceForRepository(CleanContext context)
        {
            var path = GetOutputDirectory(context.Repository);

            var size = GetDirectorySize(path);
            return size;
        }

        protected override void CleanRepository(CleanContext context)
        {
            var path = GetOutputDirectory(context.Repository);

            DeleteDirectory(path, context);
        }

        private string GetOutputDirectory(Repository repository)
        {
            var path = Path.Combine(repository.Directory, "output");
            return path;
        }
    }
}