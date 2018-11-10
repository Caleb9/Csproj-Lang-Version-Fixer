using System.Collections.Generic;
using Microsoft.Build.Evaluation;

namespace CsprojFixer.Fixer
{
    public class CompositeFixer : IFixer
    {
        private IEnumerable<IFixer> _fixers;

        public CompositeFixer(IEnumerable<IFixer> fixers)
        {
            _fixers = fixers;
        }

        public void Fix(Project project)
        {
            foreach (IFixer fixer in _fixers)
            {
                fixer.Fix(project);
            }
        }
    }
}