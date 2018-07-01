using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Exceptions;

namespace LangVersionFixer
{
    public class Fixer
    {
        private const string LangVersionPropertyName = "LangVersion";
        private const string LangVersionPropertyValue = "latest";
        
        /// <exception cref="ArgumentException">Specify project file.</exception>
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                throw new ArgumentException("Specify project file.");
            }

            /* Quickfix to https://github.com/Microsoft/msbuild/issues/2369 */
            Environment.SetEnvironmentVariable(
                "VSINSTALLDIR",
                @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional");
            Environment.SetEnvironmentVariable(
                "VisualStudioVersion",
                @"15.0");

            new Fixer().Fix(args[0]);
        }

        /// <exception cref="ArgumentException">Value cannot be null or whitespace.</exception>
        /// <exception cref="InvalidProjectFileException">If the evaluation fails.</exception>
        public void Fix(
            [NotNull] string projectFilePath)
        {
            if (string.IsNullOrWhiteSpace(projectFilePath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectFilePath));
            }

            var project = new Project(projectFilePath);
            foreach (ProjectPropertyGroupElement propertyGroupElement in GetPropertyGroupsThatNeedFixing(project))
            {
                propertyGroupElement.AddProperty(LangVersionPropertyName, LangVersionPropertyValue);
            }
            project.Save();
        }

        private IEnumerable<ProjectPropertyGroupElement> GetPropertyGroupsThatNeedFixing(
            [NotNull] Project project)
        {
            return
                project
                    .Xml
                    .PropertyGroups
                    .Where(g => HasConfigurationPlatformCondition(g) && !HasProperty(g, LangVersionPropertyName));
        }

        private bool HasConfigurationPlatformCondition(
            [NotNull] ProjectElement propertyGroup)
        {
            return propertyGroup.Condition.StartsWith(@"'$(Configuration)|$(Platform)' == ");
        }

        private bool HasProperty(
            [NotNull] ProjectPropertyGroupElement propertyGroup,
            [NotNull] string propertyName)
        {
            return propertyGroup.Properties.Any(p => p.Name == propertyName);
        }
    }
}