using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Exceptions;

namespace CsprojLangVersionFixer
{
    /// <summary>
    /// </summary>
    public sealed class Fixer
    {
        /// <summary>
        ///     Possible langVersion values.
        /// </summary>
        public enum LangVersionPropertyValues
        {
            /// <summary>
            ///     Latest major version.
            /// </summary>
            Default,

            /// <summary>
            ///     Latest minor version.
            /// </summary>
            Latest
        }

        private const string LangVersionPropertyName = "LangVersion";
        [NotNull] private readonly string _langVersionValue;

        /// <summary>
        /// </summary>
        /// <param name="langVersion"></param>
        public Fixer(LangVersionPropertyValues langVersion = LangVersionPropertyValues.Latest)
        {
            switch (langVersion)
            {
                case LangVersionPropertyValues.Default:
                    _langVersionValue = "default";
                    break;
                case LangVersionPropertyValues.Latest:
                    _langVersionValue = "latest";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(langVersion), langVersion, null);
            }
        }


        /// <exception cref="ArgumentException">Value cannot be null or whitespace.</exception>
        /// <exception cref="InvalidProjectFileException">If the evaluation fails.</exception>
        [NotNull]
        public void Fix(
            [NotNull] Project project)
        {
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            var needsNewPropertyGroup = true;

            /* First, remove all LangVersion properties with a configuration condition */
            foreach (ProjectPropertyGroupElement propertyGroupElement in
                GetPropertyGroupsWithLangVersionProperty(project))
            {
                if (propertyGroupElement.Condition == string.Empty)
                {
                    /* This property group already defines LangVersion for all configurations. We only need to set it
                     * to the requested value. */
                    needsNewPropertyGroup = false;
                    propertyGroupElement.SetProperty(LangVersionPropertyName, _langVersionValue);
                    continue;
                }

                ProjectPropertyElement langVersionProperty =
                    propertyGroupElement.Properties.Single(p => p.Name == LangVersionPropertyName);
                propertyGroupElement.RemoveChild(langVersionProperty);
            }

            if (needsNewPropertyGroup)
            {
                /* Add PropertyGroup containing LangVersion for all build configurations */
                ProjectPropertyGroupElement newPropertyGroup = project.Xml.AddPropertyGroup();
                newPropertyGroup.AddProperty(LangVersionPropertyName, _langVersionValue);
            }
        }

        private IEnumerable<ProjectPropertyGroupElement> GetPropertyGroupsWithLangVersionProperty(
            [NotNull] Project project)
        {
            return project.Xml.PropertyGroups.Where(g => g.Properties.Any(p => p.Name == LangVersionPropertyName));
        }
    }
}