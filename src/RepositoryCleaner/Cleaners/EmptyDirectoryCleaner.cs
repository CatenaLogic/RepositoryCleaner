namespace RepositoryCleaner.Cleaners
{
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using Catel.Logging;
    using Models;
    using Orc.FileSystem;

    [Cleaner("EmptyDirectoryCleaner", Description = "Delete empty directories inside subfolders of the repositories (root will stay intact)")]
    public class EmptyDirectoryCleaner : CleanerBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public EmptyDirectoryCleaner(IDirectoryService directoryService) 
            : base(directoryService)
        {
        }

        protected override bool CanCleanRepository(CleanContext context)
        {
            var directories = GetEmptyDirectories(context.Repository);
            return directories.Any();
        }

        protected override ulong CalculateCleanableSpaceForRepository(CleanContext context)
        {
            // Treat every folder as 1 byte
            var directories = GetEmptyDirectories(context.Repository);
            return (ulong)directories.Count;
        }

        protected override void CleanRepository(CleanContext context)
        {
            var directories = GetEmptyDirectories(context.Repository);

            foreach (var directory in directories)
            {
                DeleteDirectory(directory, context);
            }
        }

        private List<string> GetEmptyDirectories(Repository repository)
        {
            // Separate so we can skip root directories
            var emptyDirectories = new List<string>();

            var directories = _directoryService.GetDirectories(repository.Directory);

            foreach (var directory in directories)
            {
                // Only include a very specific set
                if (!directory.EndsWithAnyIgnoreCase("output", "obj", "lib"))
                {
                    continue;
                }

                emptyDirectories.AddRange(GetEmptyDirectories(directory));
            }

            return emptyDirectories;
        }

        private List<string> GetEmptyDirectories(string directory)
        {
            var emptyDirectories = new List<string>();

            foreach (var subDirectory in _directoryService.GetDirectories(directory))
            {
                if (_directoryService.IsEmpty(subDirectory))
                {
                    emptyDirectories.Add(subDirectory);
                }
                else
                {
                    emptyDirectories.AddRange(GetEmptyDirectories(subDirectory));
                }
            }

            return emptyDirectories;
        }
    }
}
