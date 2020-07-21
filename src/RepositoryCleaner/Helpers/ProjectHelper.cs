// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectHelper.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable 0168
// ReSharper disable UnusedVariable
//#define LOG_WARNINGS

namespace RepositoryCleaner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Catel;
    using Catel.Caching;
    using Catel.Logging;
    using Catel.Reflection;
    using Microsoft.Build.Construction;
    using Microsoft.Build.Evaluation;

    internal static class ProjectHelper
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly ICacheStorage<string, Project> ProjectsCache = new CacheStorage<string, Project>(storeNullValues: true);

        //private static readonly Type SolutionFileType;
        //private static readonly PropertyInfo SolutionReaderPropertyInfo;
        //private static readonly PropertyInfo ProjectsPropertyInfo;
        //private static readonly PropertyInfo SolutionConfigurationsPropertyInfo;
        //private static readonly MethodInfo ParseSolutionMethodInfo;
        //private static readonly PropertyInfo RelativePathPropertyInfo;
        //private static readonly PropertyInfo ProjectTypePropertyInfo;
        //private static readonly object KnownToBeMsBuildFormat;

        //static ProjectHelper()
        //{
        //    const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        //    SolutionFileType = TypeCache.GetType("Microsoft.Build.Construction.SolutionFile");
        //    if (SolutionFileType != null)
        //    {
        //        SolutionReaderPropertyInfo = SolutionFileType.GetProperty("SolutionReader", bindingFlags);
        //        SolutionConfigurationsPropertyInfo = SolutionFileType.GetProperty("SolutionConfigurations", bindingFlags);

        //        ProjectsPropertyInfo = SolutionFileType.GetProperty("Projects", bindingFlags);
        //        ParseSolutionMethodInfo = SolutionFileType.GetMethod("ParseSolution", bindingFlags);
        //    }

        //    var projectInSolutionType = TypeCache.GetType("Microsoft.Build.Construction.ProjectInSolution");
        //    if (projectInSolutionType != null)
        //    {
        //        RelativePathPropertyInfo = projectInSolutionType.GetProperty("RelativePath", bindingFlags);
        //        ProjectTypePropertyInfo = projectInSolutionType.GetProperty("ProjectType", bindingFlags);
        //    }

        //    var solutionProjectTypeType = TypeCache.GetType("Microsoft.Build.Construction.SolutionProjectType");
        //    if (solutionProjectTypeType != null)
        //    {
        //        KnownToBeMsBuildFormat = Enum.Parse(solutionProjectTypeType, "KnownToBeMSBuildFormat");
        //    }
        //}

        public static IEnumerable<KeyValuePair<string, string>> GetSolutionConfigurationsAndPlatforms(string solutionFileName)
        {
            var items = new List<KeyValuePair<string, string>>();

            //if (SolutionFileType is null)
            //{
            //    return items;
            //}

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

            foreach (var configurationAndPlatform in configurationsAndPlatforms)
            {
                projects.AddRange(GetProjects(solutionFileName, configurationAndPlatform.Key, configurationAndPlatform.Value));
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
                    if (project != null)
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
            Argument.IsNotNull(() => projectFile);
            Argument.IsNotNull(() => configurationName);
            Argument.IsNotNull(() => platformName);
            Argument.IsNotNull(() => solutionDirectory);

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

                    var project = new Project(projectFile, collections, null);
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
