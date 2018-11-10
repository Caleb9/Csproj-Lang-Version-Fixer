using JetBrains.Annotations;
using Microsoft.Build.Evaluation;

namespace CsprojFixer.Fixer
{
    public interface IFixer
    {
        void Fix( [NotNull] Project project );
    }
}