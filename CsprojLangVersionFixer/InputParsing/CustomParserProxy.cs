using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using JetBrains.Annotations;

namespace CsprojFixer.InputParsing
{
    /// <summary>
    ///     Parses command line arguments.
    /// </summary>
    public sealed class CustomParserProxy
    {
        [NotNull] private readonly TextWriter _errorOutputWriter;
        [NotNull] private readonly Parser _parser;

        /// <summary />
        /// <param name="errorOutputWriter">Errors and help message will be written here</param>
        public CustomParserProxy([NotNull] TextWriter errorOutputWriter)
        {
            _errorOutputWriter = errorOutputWriter ?? throw new ArgumentNullException(nameof(errorOutputWriter));
            _parser = new Parser(with =>
            {
                with.EnableDashDash = true;
                with.HelpWriter = errorOutputWriter;
            });
        }

        /// <summary>
        ///     Parses arguments to an instance of <see cref="Options"/>
        /// </summary>
        /// <returns>True if all arguments were valid</returns>
        public bool TryParse(
            [NotNull] string[] args,
            [NotNull] out Options options)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            Options localOptions = null;
            var success = true;
            _parser.ParseArguments<Options>(args)
                .WithNotParsed(errors => success = false)
                .WithParsed(o => localOptions = o);

            options = localOptions;
            return success;
        }

        /// <summary>
        ///     Populates list of detected .csproj file paths.
        /// </summary>
        /// <param name="inputPaths"></param>
        /// <param name="validPaths"></param>
        /// <returns>True if errors in the <paramref name="inputPaths"/> were not detected.</returns>
        public bool TrySelectCsprojFilePaths(
            [NotNull] IEnumerable<string> inputPaths,
            out IEnumerable<string> validPaths)
        {
            List<string> paths = inputPaths.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            List<string> invalidPaths =
                paths.Where(p => !p.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase)).ToList();
            validPaths = paths.Except(invalidPaths).ToList();

            return ValidateProjectPaths(validPaths, invalidPaths);
        }

        private bool ValidateProjectPaths(IEnumerable<string> csprojFiles, IEnumerable<string> invalidPaths)
        {
            IEnumerable<string> enumeratedInvalidPaths = invalidPaths.ToList();
            if (enumeratedInvalidPaths.Any())
            {
                _errorOutputWriter.WriteLine("Invalid .csproj paths specified:");
                _errorOutputWriter.WriteLine(string.Join($"{Environment.NewLine}", enumeratedInvalidPaths));
                return false;
            }

            if (!csprojFiles.Any())
            {
                _errorOutputWriter.WriteLine("No C# project files specified. Run with --help for more info.");
                return false;
            }

            return true;
        }
    }
}