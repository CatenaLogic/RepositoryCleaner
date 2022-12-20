#pragma warning disable 0168
// ReSharper disable UnusedVariable
//#define LOG_WARNINGS

namespace RepositoryCleaner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Catel;
    using Catel.Caching;
    using Catel.Logging;
    using Microsoft.Build.Construction;
    using Microsoft.Build.Evaluation;

    internal static class ProjectHelper
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly ICacheStorage<string, Project> ProjectsCache = new CacheStorage<string, Project>(storeNullValues: true);

        public static IEnumerable<KeyValuePair<string, string>> GetSolutionConfigurationsAndPlatforms(string solutionFileName)
        {
            var items = new List<KeyValuePair<string, string>>();

            try
            {
                var solutionFile = SolutionFile.Parse(solutionFileName);
               
                foreach (var solutionConfiguration in solutionFile.SolutionConfigurations)
                {
                    var configuration = solutionConfiguration.ConfigurationName;
                    var platform = solutionConfiguration.PlatformName;

                    items.Add(new KeyValuePair<string, string>(configuration, platform));

                    if (string.Equals("Any CPU", platform))
                    {
                        // Somehow there is a bug where Any CPU and AnyCPU are not treated the same, fix this
                        items.Add(new KeyValuePair<string, string>(configuration, "AnyCPU"));
                    }
                }
            }
            catch (Exception ex)
            {
#if LOG_WARNINGS
                Log.Warning(ex, "Failed to retrieve the configurations and platforms for solution '{0}'", solutionFile);
#endif
            }

            return items;
        }

        public static IEnumerable<Project> GetProjectsForAllConfigurations(string solutionFileName)
        {
            var projects = new List<Project>();

            var configurationsAndPlatforms = GetSolutionConfigurationsAndPlatforms(solutionFileName);
            if (configurationsAndPlatforms.Any())
            {
                var solutionFile = SolutionFile.Parse(solutionFileName);

                foreach (var configurationAndPlatform in configurationsAndPlatforms)
                {
                    projects.AddRange(GetProjects(solutionFileName, configurationAndPlatform.Key, configurationAndPlatform.Value));
                }
            }

            return projects;
        }

        public static IEnumerable<Project> GetProjects(string solutionFileName, string configurationName, string platformName)
        {
            var projects = new List<Project>();

            try
            {
                var solutionFile = SolutionFile.Parse(solutionFileName);
                var solutionDirectory = Path.GetDirectoryName(solutionFileName);

                foreach (var projectInSolution in solutionFile.ProjectsInOrder)
                {
                    var relativePath = projectInSolution.RelativePath;
                    var projectFile = Path.Combine(solutionDirectory, relativePath);

                    if (projectInSolution.ProjectType != SolutionProjectType.KnownToBeMSBuildFormat)
                    {
                        continue;
                    }

                    var project = LoadProject(projectFile, configurationName, platformName, solutionDirectory);
                    if (project is not null)
                    {
                        projects.Add(project);
                    }
                }
            }
            catch (Exception ex)
            {
#if LOG_WARNINGS
                Log.Warning(ex, "Failed to retrieve the projects for solution '{0}'", solutionFile);
#endif
            }

            return projects;
        }

        public static Project LoadProject(string projectFile, string configurationName, string platformName, string solutionDirectory)
        {
            ArgumentNullException.ThrowIfNull(projectFile);
            ArgumentNullException.ThrowIfNull(configurationName);
            ArgumentNullException.ThrowIfNull(platformName);
            ArgumentNullException.ThrowIfNull(solutionDirectory);

            var key = $"{projectFile}_{configurationName}_{platformName}";

            return ProjectsCache.GetFromCacheOrFetch(key, () =>
            {
                if (!solutionDirectory.EndsWith(@"\"))
                {
                    solutionDirectory += @"\";
                }

                try
                {
                    var collections = new Dictionary<string, string>();
                    collections["Configuration"] = configurationName;
                    collections["Platform"] = platformName;
                    collections["SolutionDir"] = solutionDirectory;

#pragma warning disable IDISP001 // Dispose created
                    var projectCollection = new ProjectCollection
                    {
                        
                    };
#pragma warning restore IDISP001 // Dispose created

                    var project = new Project(projectFile, collections, null, projectCollection, ProjectLoadSettings.IgnoreMissingImports | ProjectLoadSettings.IgnoreInvalidImports);
                    return project;
                }
                catch (Exception ex)
                {
#if LOG_WARNINGS
                    Log.Warning("Failed to load project '{0}': {1}", projectFile, ex.Message);
#endif
                    return null;
                }
            });
        }
    }
}
