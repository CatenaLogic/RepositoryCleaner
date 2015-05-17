// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectExtensions.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Catel;
    using Catel.Logging;
    using Microsoft.Build.Evaluation;

    public static class ProjectExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static bool ShouldBeIgnored(this Project project, IEnumerable<string> projectsToIgnore)
        {
            Argument.IsNotNull(() => project);

            var projectName = GetProjectName(project).ToLower();

            foreach (var projectToIgnore in projectsToIgnore)
            {
                var lowerCaseProjectToIgnore = projectToIgnore.ToLower();

                if (string.Equals(projectName, lowerCaseProjectToIgnore))
                {
                    return true;
                }
            }

            return false;
        }

        public static string GetProjectName(this Project project)
        {
            Argument.IsNotNull(() => project);

            var projectName = project.GetPropertyValue("MSBuildProjectName");
            return projectName ?? Path.GetFileName(project.FullPath);
        }

        public static string GetIntermediateDirectory(this Project project)
        {
            Argument.IsNotNull(() => project);

            var relativeIntermediateDirectory = GetRelativeIntermediateDirectory(project);
            var intermediateDirectory = Path.Combine(project.DirectoryPath, relativeIntermediateDirectory);
            return intermediateDirectory;
        }

        public static string GetRelativeIntermediateDirectory(this Project project)
        {
            Argument.IsNotNull(() => project);

            var projectIntermediateDirectory = project.GetPropertyValue("IntermediateOutputPath");
            return projectIntermediateDirectory;
        }

        public static string GetTargetDirectory(this Project project)
        {
            Argument.IsNotNull(() => project);

            var targetDirectory = project.GetPropertyValue("TargetDir");
            if (string.IsNullOrWhiteSpace(targetDirectory))
            {
                var relativeTargetDirectory = GetRelativeTargetDirectory(project);
                targetDirectory = Path.Combine(project.DirectoryPath, relativeTargetDirectory);
            }

            return targetDirectory;
        }

        public static string GetRelativeTargetDirectory(this Project project)
        {
            Argument.IsNotNull(() => project);

            var projectOutputDirectory = project.GetPropertyValue("OutputPath");
            if (string.IsNullOrWhiteSpace(projectOutputDirectory))
            {
                projectOutputDirectory = project.GetPropertyValue("OutDir");
            }

            return projectOutputDirectory;
        }
    }
}