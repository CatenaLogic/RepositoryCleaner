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
    using Microsoft.Build.Evaluation;

    internal static class ProjectHelper
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static ICacheStorage<string, Project> _projectsCache = new CacheStorage<string, Project>(storeNullValues: true);

        private static readonly Type SolutionParserType;
        private static readonly PropertyInfo SolutionReaderPropertyInfo;
        private static readonly PropertyInfo ProjectsPropertyInfo;
        private static readonly PropertyInfo SolutionConfigurationsPropertyInfo;
        private static readonly MethodInfo ParseSolutionMethodInfo;
        private static readonly PropertyInfo RelativePathPropertyInfo;
        private static readonly PropertyInfo ProjectTypePropertyInfo;
        private static readonly object KnownToBeMsBuildFormat;

        static ProjectHelper()
        {
            const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

            SolutionParserType = TypeCache.GetType("Microsoft.Build.Construction.SolutionParser");
            if (SolutionParserType != null)
            {
                SolutionReaderPropertyInfo = SolutionParserType.GetProperty("SolutionReader", bindingFlags);
                SolutionConfigurationsPropertyInfo = SolutionParserType.GetProperty("SolutionConfigurations", bindingFlags);

                ProjectsPropertyInfo = SolutionParserType.GetProperty("Projects", bindingFlags);
                ParseSolutionMethodInfo = SolutionParserType.GetMethod("ParseSolution", bindingFlags);
            }

            var projectInSolutionType = TypeCache.GetType("Microsoft.Build.Construction.ProjectInSolution");
            if (projectInSolutionType != null)
            {
                RelativePathPropertyInfo = projectInSolutionType.GetProperty("RelativePath", bindingFlags);
                ProjectTypePropertyInfo = projectInSolutionType.GetProperty("ProjectType", bindingFlags);
            }

            var solutionProjectTypeType = TypeCache.GetType("Microsoft.Build.Construction.SolutionProjectType");
            if (solutionProjectTypeType != null)
            {
                KnownToBeMsBuildFormat = Enum.Parse(solutionProjectTypeType, "KnownToBeMSBuildFormat");
            }
        }

        public static IEnumerable<KeyValuePair<string, string>> GetSolutionConfigurationsAndPlatforms(string solutionFile)
        {
            var items = new List<KeyValuePair<string, string>>();

            var solutionParser = SolutionParserType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(null);

            try
            {
                using (var streamReader = new StreamReader(solutionFile))
                {

                    SolutionReaderPropertyInfo.SetValue(solutionParser, streamReader, null);
                    ParseSolutionMethodInfo.Invoke(solutionParser, null);

                    var solutionConfigurations = SolutionConfigurationsPropertyInfo.GetValue(solutionParser, null);
                    var indexerPropertyInfo = solutionConfigurations.GetType().GetPropertyEx("Item");

                    for (var i = 0; i < PropertyHelper.GetPropertyValue<int>(solutionConfigurations, "Count"); i++)
                    {
                        var item = indexerPropertyInfo.GetValue(solutionConfigurations, new object[] { i });

                        var configuration = PropertyHelper.GetPropertyValue<string>(item, "ConfigurationName");
                        var platform = PropertyHelper.GetPropertyValue<string>(item, "PlatformName");

                        items.Add(new KeyValuePair<string, string>(configuration, platform));

                        if (string.Equals("Any CPU", platform))
                        {
                            // Somehow there is a bug where Any CPU and AnyCPU are not treated the same, fix this
                            items.Add(new KeyValuePair<string, string>(configuration, "AnyCPU"));
                        }
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

        public static IEnumerable<Project> GetProjectsForAllConfigurations(string solutionFile)
        {
            var projects = new List<Project>();

            var configurationsAndPlatforms = GetSolutionConfigurationsAndPlatforms(solutionFile);

            foreach (var configurationAndPlatform in configurationsAndPlatforms)
            {
                projects.AddRange(GetProjects(solutionFile, configurationAndPlatform.Key, configurationAndPlatform.Value));
            }

            return projects;
        }

        public static IEnumerable<Project> GetProjects(string solutionFile, string configurationName, string platformName)
        {
            var projects = new List<Project>();
            var solutionParser = SolutionParserType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(null);

            try
            {
                using (var streamReader = new StreamReader(solutionFile))
                {

                    SolutionReaderPropertyInfo.SetValue(solutionParser, streamReader, null);
                    ParseSolutionMethodInfo.Invoke(solutionParser, null);
                    var solutionDirectory = Path.GetDirectoryName(solutionFile);
                    var array = (Array)ProjectsPropertyInfo.GetValue(solutionParser, null);
                    for (var i = 0; i < array.Length; i++)
                    {
                        var projectInSolution = array.GetValue(i);
                        if (!ObjectHelper.AreEqual(ProjectTypePropertyInfo.GetValue(projectInSolution), KnownToBeMsBuildFormat))
                        {
                            continue;
                        }

                        var relativePath = (string)RelativePathPropertyInfo.GetValue(projectInSolution);
                        var projectFile = Path.Combine(solutionDirectory, relativePath);

                        var project = LoadProject(projectFile, configurationName, platformName, solutionDirectory);
                        if (project != null)
                        {
                            projects.Add(project);
                        }
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
            Argument.IsNotNullOrWhitespace(() => projectFile);
            Argument.IsNotNullOrWhitespace(() => configurationName);
            Argument.IsNotNullOrWhitespace(() => platformName);
            Argument.IsNotNullOrWhitespace(() => solutionDirectory);

            var key = string.Format("{0}_{1}_{2}", projectFile, configurationName, platformName);

            return _projectsCache.GetFromCacheOrFetch(key, () =>
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