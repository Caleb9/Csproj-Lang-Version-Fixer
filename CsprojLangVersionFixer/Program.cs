using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Evaluation;

namespace CsprojLangVersionFixer
{
    /// <summary>
    /// </summary>
    public static class Program
    {
        public static int Main(string[] args)
        {
            CommandLineArgumentsParser commandLineArguments;
            try
            {
                commandLineArguments = CommandLineArgumentsParser.Parse(args);
            }
            catch (ArgumentException e)
            {
                Console.Error.WriteLine(e);
                return 1;
            }

            if (!commandLineArguments.ProjectFilePaths.Any() || commandLineArguments.Help)
            {
                Console.WriteLine(CommandLineArgumentsParser.HelpMessage);
                return 0;
            }

            ProcessFiles(commandLineArguments.ProjectFilePaths, commandLineArguments.DryRun);

            return 0;
        }

        private static void ProcessFiles(
            IEnumerable<string> projectFilePaths,
            bool dryRun = false)
        {
            var fixer = new Fixer();
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
                    Console.Error.WriteLine($"{projectFile}: {e}");
                }
            }
        }
    }
}