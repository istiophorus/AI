using System;
using System.IO;

namespace Archer
{
    public sealed class NetworkSolutionProvider : IProblemSolver
    {
        public NetworkSolutionProvider(string networkPath)
        {
            if (null == networkPath)
            {
                throw new ArgumentNullException(nameof(networkPath));
            }

            if (!File.Exists(networkPath))
            {
                throw new FileNotFoundException(networkPath);
            }
        }

        public ProblemDefinition ResolveProblem(TargetParameters targetParameters)
        {
            if (null == targetParameters)
            {
                throw new ArgumentNullException(nameof(targetParameters));
            }

            return null;
        }
    }
}
