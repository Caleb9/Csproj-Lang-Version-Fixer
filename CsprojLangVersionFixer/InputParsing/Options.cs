using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommandLine;
using CommandLine.Text;
using JetBrains.Annotations;

namespace CsprojFixer.InputParsing
{
    /// <summary>
    ///     Options created from parsed command line arguments. Uses <see cref="CommandLine" /> library.
    /// </summary>
    public sealed class Options
    {
        /// <summary />
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public Options(
            string langVersion,
            string targetFrameworkVersion,
            [NotNull] IEnumerable<string> projectFilePaths,
            bool dryRun = false)
        {
            LangVersion = langVersion;
            TargetFrameworkVersion = targetFrameworkVersion;
            ProjectFilePaths = projectFilePaths ?? throw new ArgumentNullException(nameof(projectFilePaths));
            DryRun = dryRun;
        }

        [Option('l', "lang-version", HelpText = "LangVersion")]
        public string LangVersion { get; }

        [Option('f', "framework", HelpText="TargetFrameworkVersion")]
        public string TargetFrameworkVersion { get; }

        /// <summary>
        ///     Detected .csproj file paths.
        /// </summary>
        [NotNull]
        // TODO Min disabled until https://github.com/commandlineparser/commandline/issues/363 is answered
        [Value(0, /*Min = 1,*/ HelpText = "Path(s) to .csproj file(s)", Hidden = true)]
        public IEnumerable<string> ProjectFilePaths { get; }

        /// <summary>
        ///     Indicates that files should not be modified.
        /// </summary>
        [Option('d', "dry-run", Default = false, HelpText = "dry run (does not modify files)")]
        public bool DryRun { get; }

        /// <summary>
        /// </summary>
        [Usage(ApplicationAlias = "$ CsprojLangVersionFixer.exe")]
        [UsedImplicitly]
        public static IEnumerable<Example> Usage
        {
            get
            {
                yield return new Example("Path to .csproj file to be fixed",
                    new Options(string.Empty, string.Empty, @"C:\myproject\myproject.csproj".Split()));
                yield return new Example("Paths to multiple .csproj files to be fixed",
                    new Options(string.Empty, string.Empty, @"C:\project1\project1.csproj C:\project2\project2.csproj".Split()));
                yield return new Example("Path to .csproj file to be checked (without modifying)",
                    new Options(string.Empty, string.Empty, @"-d C:\myproject\myproject1.csproj".Split()));
            }
        }
    }
}