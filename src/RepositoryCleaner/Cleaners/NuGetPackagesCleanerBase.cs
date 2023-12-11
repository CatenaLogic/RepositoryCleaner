namespace RepositoryCleaner.Cleaners
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Catel.Logging;
    using Models;
    using Orc.FileSystem;

    public abstract class NuGetPackagesCleanerBase : CleanerBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        protected NuGetPackagesCleanerBase(IDirectoryService directoryService) 
            : base(directoryService)
        {
        }

        protected abstract string GetPackagesDirectory(Repository repository);

        protected override bool CanCleanRepository(CleanContext context)
        {
            var libPath = GetPackagesDirectory(context.Repository);

            if (!_directoryService.Exists(libPath))
            {
                return false;
            }

            foreach (var directory in _directoryService.GetDirectories(libPath))
            {
                if (IsCleanablePath(directory))
                {
                    return true;
                }
            }

            return false;
        }

        protected override ulong CalculateCleanableSpaceForRepository(CleanContext context)
        {
            ulong space = 0L;

            foreach (var directory in GetNuGetPackageDirectories(context.Repository))
            {
                space += GetDirectorySize(directory);
            }

            return space;
        }

        protected override void CleanRepository(CleanContext context)
        {
            foreach (var directory in GetNuGetPackageDirectories(context.Repository))
            {
                if (IsCleanablePath(directory))
                {
                    DeleteDirectory(directory, context);
                }
            }
        }

        private IEnumerable<string> GetPossibleNuGetPackageDirectories(string rootPath)
        {
            return Directory.GetDirectories(rootPath);
        }

        private IEnumerable<string> GetNuGetPackageDirectories(Repository repository)
        {
            var packagesDirectory = GetPackagesDirectory(repository);

            return (from directory in GetPossibleNuGetPackageDirectories(packagesDirectory)
                    where IsCleanablePath(directory)
                    select directory);
        }

        private bool IsCleanablePath(string path)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }

            return Directory.GetFiles(path, "*.nupkg", SearchOption.AllDirectories).Any();
        }
    }
}
