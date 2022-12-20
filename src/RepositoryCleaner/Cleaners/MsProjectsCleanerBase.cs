namespace RepositoryCleaner.Cleaners
{
    using System.Collections.Generic;
    using System.IO;
    using Catel.Caching;
    using Microsoft.Build.Evaluation;
    using Models;
    using Orc.FileSystem;

    public abstract class MsProjectsCleanerBase : CleanerBase
    {
        private static readonly ICacheStorage<string, IEnumerable<Project>> SolutionsCache = new CacheStorage<string, IEnumerable<Project>>();

        protected MsProjectsCleanerBase(IDirectoryService directoryService) 
            : base(directoryService)
        {
        }

        protected virtual IEnumerable<Project> GetAllProjects(Repository repository)
        {
            var solutionFiles = _directoryService.GetFiles(repository.Directory, "*.sln", SearchOption.AllDirectories);

            var projects = new List<Project>();

            foreach (var solutionFile in solutionFiles)
            {
                var solutionProjects = GetAllProjects(solutionFile);
                projects.AddRange(solutionProjects);
            }

            return projects;
        }

        protected virtual IEnumerable<Project> GetAllProjects(string solutionFile)
        {
            return SolutionsCache.GetFromCacheOrFetch(solutionFile, () =>
            {
                var projects = ProjectHelper.GetProjectsForAllConfigurations(solutionFile);
                return projects;
            });
        }
    }
}
