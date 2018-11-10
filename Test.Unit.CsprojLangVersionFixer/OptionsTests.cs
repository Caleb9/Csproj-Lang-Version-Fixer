using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommandLine;
using CsprojFixer.InputParsing;
using NUnit.Framework;

namespace Test.Unit.CsprojLangVersionFixer
{
    internal static class ParserExtensions
    {
        /// <summary>
        ///     Helper extension methods to obtain instance of <typeparamref name="TOptions" /> from a
        ///     <see cref="Parser"/>.
        /// </summary>
        public static TOptions ParseToOptions<TOptions>(this Parser parser, string[] args)
        {
            TOptions result = default;
            parser.ParseArguments<TOptions>(args).WithParsed(options => result = options);
            return result;
        }
    }

    [TestFixture]
    [TestOf(typeof(Options))]
    [ExcludeFromCodeCoverage]
    [SuppressMessage("ReSharper", "ExceptionNotDocumented")]
    public class OptionsTests
    {
        private readonly Parser _sut = new Parser(with => with.EnableDashDash = true);

        [Test]
        public void ProjectOnly_ResultHasOneProject()
        {
            /* Arrange */
            var args = "-- myproject.csproj".Split();

            /* Act */
            var result = _sut.ParseToOptions<Options>(args);

            /* Assert */
            Assert.AreEqual(result.ProjectFilePaths.Count(), 1);
        }

        [Test]
        public void ProjectOnly_ResultContainsProject()
        {
            /* Arrange */
            var args = @"-- C:\project\myproject.csproj".Split();

            /* Act */
            var result = _sut.ParseToOptions<Options>(args);

            /* Assert */
            CollectionAssert.Contains(result.ProjectFilePaths, @"C:\project\myproject.csproj");
        }

        [Test]
        public void DryRun_ResultDryRunIsTrue()
        {
            /* Arrange */
            var args = "-d -- myproject.csproj".Split();

            /* Act */
            var result = _sut.ParseToOptions<Options>(args);

            /* Assert */
            Assert.IsTrue(result.DryRun);
        }

        [Test]
        public void NoDryRun_ResultDryRunIsFalse()
        {
            /* Arrange */
            var args = "-- myproject.csproj".Split();

            /* Act */
            var result = _sut.ParseToOptions<Options>(args);

            /* Assert */
            Assert.IsFalse(result.DryRun);
        }

        [Test]
        public void TwoProjects_ResultHasTwoProjects()
        {
            /* Arrange */
            var args = "-- myproject1.csproj myproject2.csproj".Split();

            /* Act */
            var result = _sut.ParseToOptions<Options>(args);

            /* Assert */
            Assert.AreEqual(result.ProjectFilePaths.Count(), 2);
        }

    }
}