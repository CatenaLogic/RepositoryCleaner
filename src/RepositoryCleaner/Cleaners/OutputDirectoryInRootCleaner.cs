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

    [Cleaner("OutputDirectoryInRootCleaner", Description = "Delete output directory in the root (thus [repository]\\output")]
    public class OutputDirectoryInRootCleaner : MsProjectsCleanerBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public OutputDirectoryInRootCleaner(IDirectoryService directoryService) 
            : base(directoryService)
        {
        }

        protected override bool CanCleanRepository(CleanContext context)
        {
            var projects = GetAllProjects(context.Repository);
            return projects.Any();
        }

        protected override long CalculateCleanableSpaceForRepository(CleanContext context)
        {
            var size = 0L;

            var projects = GetAllProjects(context.Repository);
            var handledDirectories = new HashSet<string>();

            foreach (var project in projects)
            {
                var directory = project.GetTargetDirectory();
                if (string.IsNullOrWhiteSpace(directory))
                {
                    continue;
                }

                if (handledDirectories.Contains(directory))
                {
                    continue;
                }

                handledDirectories.Add(directory);

                size += GetDirectorySize(directory);
            }

            return size;
        }

        protected override void CleanRepository(CleanContext context)
        {
            var projects = GetAllProjects(context.Repository);

            foreach (var project in projects)
            {
                var intermediateDirectory = project.GetTargetDirectory();

                DeleteDirectory(intermediateDirectory, context);
            }
        }
    }
}