using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using CsprojFixer.Fixer;
using Microsoft.Build.Evaluation;
using NUnit.Framework;

namespace Test.Unit.CsprojLangVersionFixer
{
    /// <summary>
    ///     These are practically integration tests since they depend so much on Microsoft.Build.
    /// </summary>
    [TestFixture]
    [TestOf(typeof(GenericFixer))]
    [ExcludeFromCodeCoverage]
    public class FixerTests
    {
        private Project CreateFakeProjectFromXml(string propertyGroupXml = null)
        {
            string projectXml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""15.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
    {propertyGroupXml}
</Project>
";

            XmlReader xmlReader = XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(projectXml)));
            return new Project(xmlReader);
        }

        private GenericFixer CreateLangVersionFixerSut()
        {
            return new GenericFixer("LangVersion", "latest");
        }

        [Test]
        [Description(nameof(GenericFixer.Fix))]
        public void Fix_ProjectContainsConditionalLangVersion_ConditionalLangVersionIsRemoved()
        {
            /* Arrange */
            const string unconditionalPropertyGroup =
                @"
                <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
                    <LangVersion>default</LangVersion>
                </PropertyGroup>";
            Project project = CreateFakeProjectFromXml(unconditionalPropertyGroup);
            GenericFixer sut = CreateLangVersionFixerSut();

            /* Act */
            sut.Fix(project);

            /* Assert */
            Assert.IsFalse(
                project
                    .Xml.PropertyGroups.SingleOrDefault(g => g.Condition != string.Empty)?
                    .Properties.Any(p => p.Name == "LangVersion"));
        }

        [Test]
        [Description(nameof(GenericFixer.Fix))]
        public void Fix_ProjectContainsConditionalLangVersion_UnconditionalLangVersionIsAdded()
        {
            /* Arrange */
            const string unconditionalPropertyGroup =
                @"
                <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
                    <LangVersion>default</LangVersion>
                </PropertyGroup>";
            Project project = CreateFakeProjectFromXml(unconditionalPropertyGroup);
            GenericFixer sut = CreateLangVersionFixerSut();

            /* Act */
            sut.Fix(project);

            /* Assert */
            Assert.NotNull(
                project
                    .Xml.PropertyGroups.SingleOrDefault(g => g.Condition == string.Empty)?
                    .Properties.SingleOrDefault(p => p.Name == "LangVersion" && p.Value == "latest"));
        }

        [Test]
        [Description(nameof(GenericFixer.Fix))]
        public void Fix_ProjectContainsUnconditionalLangVersion_ProjectIsNotModified()
        {
            /* Arrange */
            const string unconditionalPropertyGroup =
                @"<PropertyGroup><LangVersion>latest</LangVersion></PropertyGroup>";
            Project project = CreateFakeProjectFromXml(unconditionalPropertyGroup);
            Debug.Assert(!project.IsDirty);
            GenericFixer sut = CreateLangVersionFixerSut();

            /* Act */
            sut.Fix(project);

            /* Assert */
            Assert.IsFalse(project.IsDirty);
        }

        [Test]
        [Description(nameof(GenericFixer.Fix))]
        public void Fix_ProjectContainsWrongLangVersion_LangVersionIsSet()
        {
            /* Arrange */
            const string unconditionalPropertyGroup =
                @"<PropertyGroup><LangVersion>default</LangVersion></PropertyGroup>";
            Project project = CreateFakeProjectFromXml(unconditionalPropertyGroup);
            GenericFixer sut = CreateLangVersionFixerSut();

            /* Act */
            sut.Fix(project);

            /* Assert */
            Assert.NotNull(
                project
                    .Xml.PropertyGroups.SingleOrDefault()?
                    .Properties.SingleOrDefault(p => p.Name == "LangVersion" && p.Value == "latest"));
        }

        [Test]
        [Description(nameof(GenericFixer.Fix))]
        public void Fix_ProjectDoesNotContainLangVersion_LangVersionIsAdded()
        {
            /* Arrange */
            Project project = CreateFakeProjectFromXml();
            GenericFixer sut = CreateLangVersionFixerSut();

            /* Act */
            sut.Fix(project);

            /* Assert */
            Assert.NotNull(
                project
                    .Xml.PropertyGroups.SingleOrDefault()?
                    .Properties.SingleOrDefault(p => p.Name == "LangVersion" && p.Value == "latest"));
        }
    }
}