namespace RepositoryCleaner
{
    using System;
    using System.IO;
    using Catel.Logging;
    using Microsoft.Build.Evaluation;
    using Microsoft.Extensions.Logging;

    public static class ProjectExtensions
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(ProjectExtensions));

        public static string GetProjectName(this Project project)
        {
            ArgumentNullException.ThrowIfNull(project);

            var projectName = project.GetPropertyValue("MSBuildProjectName");
            return projectName ?? Path.GetFileName(project.FullPath);
        }

        public static string GetIntermediateDirectory(this Project project)
        {
            ArgumentNullException.ThrowIfNull(project);

            var relativeIntermediateDirectory = GetRelativeIntermediateDirectory(project);
            if (relativeIntermediateDirectory is null)
            {
                return null;
            }

            var intermediateDirectory = Path.Combine(project.DirectoryPath, relativeIntermediateDirectory);
            return intermediateDirectory;
        }

        public static string GetRelativeIntermediateDirectory(this Project project)
        {
            ArgumentNullException.ThrowIfNull(project);

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
            ArgumentNullException.ThrowIfNull(project);

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
            ArgumentNullException.ThrowIfNull(project);

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
            Logger.LogDebug(string.Empty);
            Logger.LogDebug("Properties for project {ProjectPath}", project.FullPath);
            Logger.LogDebug("-----------------------------------------------------------");

            foreach (var property in project.Properties)
            {
                Logger.LogDebug("  {PropertyName} => {EvaluatedValue} ({UnevaluatedValue})", property.Name, property.EvaluatedValue, property.UnevaluatedValue);
            }

            Logger.LogDebug(string.Empty);
            Logger.LogDebug(string.Empty);
        }
    }
}
