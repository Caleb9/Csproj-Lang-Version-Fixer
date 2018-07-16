using System;
using CsprojLangVersionFixer;
using NUnit.Framework;

namespace Test.Unit.CsprojLangVersionFixer
{
    [TestFixture]
    [TestOf(typeof(CommandLineArgumentsParser))]
    public class CommandLineArgumentsParserTests
    {
        [Test]
        [Description(nameof(CommandLineArgumentsParser.Parse))]
        public void Parse_ArgsContainsInvalidElements_Throws()
        {
            /* Arrange */
            var args = new[] {"abc", @"C:\project1.csproj", "def", @"C:\project2.csproj"};

            /* Act */
            void TestDelegate() => CommandLineArgumentsParser.Parse(args);

            /* Assert */
            Assert.Throws<ArgumentException>(TestDelegate);
        }

        [Test]
        [Description(nameof(CommandLineArgumentsParser.Parse))]
        public void Parse_ArgsContainsSwitches_ResultProjectFilePathsContainsOnlyPaths()
        {
            /* Arrange */
            var args = new[] {"-h", @"C:\project1.csproj", "-d", @"C:\project2.csproj"};

            /* Act */
            CommandLineArgumentsParser result = CommandLineArgumentsParser.Parse(args);

            /* Assert */
            CollectionAssert.AreEquivalent(
                new[] {@"C:\project1.csproj", @"C:\project2.csproj"},
                result.ProjectFilePaths);
        }

        [Test]
        [Description(nameof(CommandLineArgumentsParser.Parse))]
        public void Parse_ContainsDryRunSwitch_ResultDryRunIsTrue()
        {
            /* Arrange */
            var args = new[] {@"C:\project1.csproj", "-d", @"C:\project2.csproj"};

            /* Act */
            CommandLineArgumentsParser result = CommandLineArgumentsParser.Parse(args);

            /* Assert */
            Assert.IsTrue(result.DryRun);
        }

        [Test]
        [Description(nameof(CommandLineArgumentsParser.Parse))]
        public void Parse_ContainsHelpSwitch_ResultHelpIsTrue()
        {
            /* Arrange */
            var args = new[] {@"C:\project1.csproj", "-h", @"C:\project2.csproj"};

            /* Act */
            CommandLineArgumentsParser result = CommandLineArgumentsParser.Parse(args);

            /* Assert */
            Assert.IsTrue(result.Help);
        }

        [Test]
        [Description(nameof(CommandLineArgumentsParser.Parse))]
        public void Parse_DoesNotContainDryRunSwitch_ResultDryRunIsFalse()
        {
            /* Arrange */
            var args = new[] {@"C:\project1.csproj", @"C:\project2.csproj"};

            /* Act */
            CommandLineArgumentsParser result = CommandLineArgumentsParser.Parse(args);

            /* Assert */
            Assert.IsFalse(result.DryRun);
        }

        [Test]
        [Description(nameof(CommandLineArgumentsParser.Parse))]
        public void Parse_DoesNotContainHelpSwitch_ResultHelpIsFalse()
        {
            /* Arrange */
            var args = new[] {@"C:\project1.csproj", @"C:\project2.csproj"};

            /* Act */
            CommandLineArgumentsParser result = CommandLineArgumentsParser.Parse(args);

            /* Assert */
            Assert.IsFalse(result.Help);
        }
    }
}