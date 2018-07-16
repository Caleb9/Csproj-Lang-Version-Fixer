﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace CsprojLangVersionFixer
{
    /// <summary>
    /// </summary>
    public sealed class CommandLineArgumentsParser
    {
        private const string DryRunSwitch = "-d";
        private const string HelpSwitch = "-h";

        private static readonly string[] Switches = {DryRunSwitch, HelpSwitch};

        /// <summary>
        /// </summary>
        public static readonly string HelpMessage = $@"
Specify .csproj files to be fixed.
Options:
    {DryRunSwitch} - dry run (does not modify files)
    {HelpSwitch} - this help message";

        private CommandLineArgumentsParser(
            [NotNull] ISet<string> projectFilePaths,
            bool dryRun = false,
            bool help = false)
        {
            ProjectFilePaths = projectFilePaths ?? throw new ArgumentNullException(nameof(projectFilePaths));
            DryRun = dryRun;
            Help = help;
        }

        /// <summary>
        ///     Indicates that files should not be modified.
        /// </summary>
        public bool DryRun { get; }

        /// <summary>
        ///     Indicates that arguments contain a help switch.
        /// </summary>
        public bool Help { get; }

        /// <summary>
        ///     Detected .csproj file paths.
        /// </summary>
        [NotNull]
        public ISet<string> ProjectFilePaths { get; }


        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        /// <returns>New instance of <see cref="CommandLineArgumentsParser" /></returns>
        [NotNull]
        public static CommandLineArgumentsParser Parse(string[] args)
        {
            List<string> validArgs =
                args
                    .Where(s => Switches.Contains(s) || s.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
                    .ToList();
            if (validArgs.Count != args.Length)
            {
                throw new ArgumentException("Invalid parameters detected.");
            }

            return new CommandLineArgumentsParser(
                validArgs.Where(s => !Switches.Contains(s)).ToHashSet(),
                args.Contains(DryRunSwitch),
                validArgs.Contains(HelpSwitch));
        }
    }
}