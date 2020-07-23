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
            if (relativeIntermediateDirectory == null)
            {
                return null;
            }

            var intermediateDirectory = Path.Combine(project.DirectoryPath, relativeIntermediateDirectory);
            return intermediateDirectory;
        }

        public static string GetRelativeIntermediateDirectory(this Project project)
        {
            Argument.IsNotNull(() => project);

            var projectIntermediateDirectory = project.GetPropertyValue("IntermediateOutputPath");
            if (!string.IsNullOrWhiteSpace(projectIntermediateDirectory))
            {
                return projectIntermediateDirectory;
            }

            var configuration = project.GetPropertyValue("Configuration");
            if (!string.IsNullOrWhiteSpace(configuration))
            {
                // Note: assume obj, we want to clean that anyway
                return "obj";

                //projectIntermediateDirectory = string.Format("obj\\{0}", configuration);
                //return projectIntermediateDirectory;
            }

            return null;
        }

        public static string GetTargetDirectory(this Project project)
        {
            Argument.IsNotNull(() => project);

            var targetDirectory = project.GetPropertyValue("TargetDir");
            if (string.IsNullOrWhiteSpace(targetDirectory))
            {
                var relativeTargetDirectory = GetRelativeTargetDirectory(project);
                if (string.IsNullOrWhiteSpace(relativeTargetDirectory))
                {
                    return null;
                }

                targetDirectory = Path.Combine(project.DirectoryPath, relativeTargetDirectory);
            }

            return targetDirectory;
        }

        public static string GetRelativeTargetDirectory(this Project project)
        {
            Argument.IsNotNull(() => project);

            var projectOutputDirectory = project.GetPropertyValue("OutputPath");
            if (!string.IsNullOrWhiteSpace(projectOutputDirectory))
            {
                return projectOutputDirectory;
            }

            projectOutputDirectory = project.GetPropertyValue("OutDir");
            if (!string.IsNullOrWhiteSpace(projectOutputDirectory))
            {
                return projectOutputDirectory;
            }

            var configuration = project.GetPropertyValue("Configuration");
            if (!string.IsNullOrWhiteSpace(configuration))
            {
                // Note: assume bin, we want to clean that anyway
                return "bin";

                //projectOutputDirectory = string.Format("bin\\{0}", configuration);
                //return projectOutputDirectory;
            }

            return null;
        }

        public static void DumpProperties(this Project project)
        {
            Log.Debug("");
            Log.Debug("Properties for project '{0}'", project.FullPath);
            Log.Debug("-----------------------------------------------------------");

            foreach (var property in project.Properties)
            {
                Log.Debug("  {0} => {1} ({2})", property.Name, property.EvaluatedValue, property.UnevaluatedValue);
            }

            Log.Debug("");
            Log.Debug("");
        }
    }
}