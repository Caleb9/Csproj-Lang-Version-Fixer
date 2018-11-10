using System;
using System.Collections.Generic;
using CsprojFixer.Fixer;
using CsprojFixer.InputParsing;
using Microsoft.Build.Evaluation;

namespace CsprojFixer
{
    /// <summary>
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// </summary>
        public static int Main(string[] args)
        {
            var parser = new CustomParserProxy(Console.Error);
            IEnumerable<string> validPaths = null;
            if (!parser.TryParse(args, out Options options) ||
                !parser.TrySelectCsprojFilePaths(options.ProjectFilePaths, out validPaths))
            {
                Environment.Exit(1);
            }

            IFixer fixer = null;
            try
            {
                fixer = FixerImplementationSelector.SelectImplementation(options);
            }
            catch (InvalidOperationException e)
            {
                Console.Error.WriteLine(e.Message);
                Environment.Exit(1);
            }
            ProcessFiles(fixer, validPaths, options.DryRun);

            return 0;
        }

        private static void ProcessFiles(
            IFixer fixer,
            IEnumerable<string> projectFilePaths,
            bool dryRun = false)
        {
            foreach (string projectFile in projectFilePaths)
            {
                try
                {
                    var project = new Project(projectFile);

                    fixer.Fix(project);

                    if (!project.IsDirty)
                    {
                        Console.WriteLine($"{projectFile}: no action necessary");
                        continue;
                    }

                    if (!dryRun)
                    {
                        project.Save();
                        Console.WriteLine($"{projectFile}: updated");
                    }
                    else
                    {
                        Console.WriteLine($"{projectFile}: would update");
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"{projectFile}: {e.Message}");
                }
            }
        }
    }
}