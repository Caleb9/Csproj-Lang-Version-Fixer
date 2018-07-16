using System;
using System.Linq;
using Microsoft.Build.Evaluation;

namespace CsprojLangVersionFixer
{
    /// <summary>
    /// </summary>
    public static class Program
    {
        /// <exception cref="ArgumentException">Specify project file.</exception>
        public static int Main(string[] args)
        {
            /* Quickfix to https://github.com/Microsoft/msbuild/issues/2369 */
            Environment.SetEnvironmentVariable(
                "VSINSTALLDIR",
                @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional");
            Environment.SetEnvironmentVariable(
                "VisualStudioVersion",
                @"15.0");

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

            ProcessFiles(commandLineArguments);

            return 0;
        }

        private static void ProcessFiles(CommandLineArgumentsParser commandLineArguments)
        {
            var fixer = new Fixer();
            foreach (string projectFile in commandLineArguments.ProjectFilePaths)
            {
                try
                {
                    Project project = fixer.Fix(projectFile);

                    if (!project.IsDirty)
                    {
                        Console.WriteLine($"{projectFile}: no action necessary");
                        continue;
                    }

                    if (!commandLineArguments.DryRun)
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