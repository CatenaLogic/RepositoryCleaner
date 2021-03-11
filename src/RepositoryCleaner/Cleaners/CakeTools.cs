// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntermediateFilesCleaner.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.Cleaners
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Catel.Logging;
    using Models;
    using Orc.FileSystem;

    [Cleaner("CakeToolsCleaner", Description = "Delete tools in the .\tools directory if Cake is detected")]
    public class CakeToolsCleaner : CleanerBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public CakeToolsCleaner(IDirectoryService directoryService) 
            : base(directoryService)
        {
        }

        protected override bool CanCleanRepository(CleanContext context)
        {
            var toolsDirectory = Path.Combine(context.Repository.Directory, "tools");

            var cakeDirectory = Path.Combine(toolsDirectory, "Cake");
            if (_directoryService.Exists(cakeDirectory))
            {
                return true;
            }

            var cakeCoreClrDirectory = Path.Combine(toolsDirectory, "Cake.CoreCLR");
            if (_directoryService.Exists(cakeCoreClrDirectory))
            {
                return true;
            }

            return false;
        }

        protected override ulong CalculateCleanableSpaceForRepository(CleanContext context)
        {
            ulong size = 0L;

            var cleanableDirectories = GetCleanableDirectories(context);

            foreach (var cleanableDirectory in cleanableDirectories)
            {
                size += GetDirectorySize(cleanableDirectory);
            }

            return size;
        }

        protected override void CleanRepository(CleanContext context)
        {
            var cleanableDirectories = GetCleanableDirectories(context);

            foreach (var cleanableDirectory in cleanableDirectories)
            {
                DeleteDirectory(cleanableDirectory, context);
            }
        }

        private List<string> GetCleanableDirectories(CleanContext context)
        {
            var directories = new List<string>();

            var toolsDirectory = Path.Combine(context.Repository.Directory, "tools");

            var cakeDirectory = Path.Combine(toolsDirectory, "Cake");
            var cakeCoreClrDirectory = Path.Combine(toolsDirectory, "Cake.CoreCLR");
            if (!_directoryService.Exists(cakeDirectory) && 
                !_directoryService.Exists(cakeCoreClrDirectory))
            {
                return directories;
            }

            var subDirectories = _directoryService.GetDirectories(toolsDirectory);
            return subDirectories.ToList();
        }
    }
}
