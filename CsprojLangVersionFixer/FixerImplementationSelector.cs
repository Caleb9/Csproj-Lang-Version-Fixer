using System;
using CsprojFixer.Fixer;
using CsprojFixer.InputParsing;

namespace CsprojFixer
{
    public sealed class FixerImplementationSelector
    {
        private const string LangVersionPropertyName = "LangVersion";
        private const string TargetFrameworkVersionPropertyName = "TargetFrameworkVersionProperty";

        public static IFixer SelectImplementation(Options options)
        {
            if (!string.IsNullOrWhiteSpace(options.LangVersion) &&
                !string.IsNullOrWhiteSpace(options.TargetFrameworkVersion))
            {
                GenericFixer langVersionFixer = CreateLangVersionFixer(options);
                GenericFixer targetFrameworkFixer = CreateTargetFrameworkFixer(options);
                return new CompositeFixer(new[] {langVersionFixer, targetFrameworkFixer});
            }

            if (!string.IsNullOrWhiteSpace(options.LangVersion))
            {
                return CreateLangVersionFixer(options);
            }

            if (!string.IsNullOrWhiteSpace(options.TargetFrameworkVersion))
            {
                return CreateTargetFrameworkFixer(options);
            }

            throw new InvalidOperationException("Either --lang-version or --target-framework option must be specified");
        }

        private static GenericFixer CreateTargetFrameworkFixer(Options options)
        {
            return new GenericFixer(TargetFrameworkVersionPropertyName, options.TargetFrameworkVersion);
        }

        private static GenericFixer CreateLangVersionFixer(Options options)
        {
            return new GenericFixer(LangVersionPropertyName, options.LangVersion);
        }
    }
}