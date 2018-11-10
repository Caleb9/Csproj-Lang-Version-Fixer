using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Exceptions;

namespace CsprojFixer.Fixer
{
    /// <summary>
    /// </summary>
    public sealed class GenericFixer: IFixer
    {
        [NotNull] private readonly string _propertyName;
        [NotNull] private readonly string _propertyValue;

        /// <summary>
        /// </summary>
        public GenericFixer([NotNull] string propertyName, [NotNull] string propertyValue)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(propertyName));
            }

            if (string.IsNullOrWhiteSpace(propertyValue))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(propertyValue));
            }

            _propertyName = propertyName;//"TargetFrameworkVersion";
            _propertyValue = propertyValue;//"v4.7.2";
        }


        /// <exception cref="ArgumentException">Value cannot be null or whitespace.</exception>
        /// <exception cref="InvalidProjectFileException">If the evaluation fails.</exception>
        public void Fix(
            [NotNull] Project project)
        {
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            var needsNewPropertyGroup = true;

            /* First, remove all properties to replace with a configuration condition */
            foreach (ProjectPropertyGroupElement propertyGroupElement in
                GetPropertyGroupsWithPropertyToReplace(project))
            {
                if (propertyGroupElement.Condition == string.Empty)
                {
                    /* This property group already defines the property for all configurations. We only need to set it
                     * to the requested value. */
                    needsNewPropertyGroup = false;
                    propertyGroupElement.SetProperty(_propertyName, _propertyValue);
                    continue;
                }

                ProjectPropertyElement property =
                    propertyGroupElement.Properties.Single(p => p.Name == _propertyName);
                propertyGroupElement.RemoveChild(property);
            }

            if (needsNewPropertyGroup)
            {
                /* Add PropertyGroup containing LangVersion for all build configurations */
                ProjectPropertyGroupElement newPropertyGroup = project.Xml.AddPropertyGroup();
                newPropertyGroup.AddProperty(_propertyName, _propertyValue);
            }
        }

        private IEnumerable<ProjectPropertyGroupElement> GetPropertyGroupsWithPropertyToReplace(
            [NotNull] Project project)
        {
            return project.Xml.PropertyGroups.Where(g =>
                g.Properties.Any(p => p.Name == _propertyName));
        }
    }
}