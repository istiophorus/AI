using Accord.Neuro;
using System;
using System.IO;

namespace Archer
{
    public sealed class NetworkSolutionProvider : IProblemSolver
    {
        private readonly Network _network;

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

            _network = ActivationNetwork.Load(networkPath);
        }

        public ProblemDefinition ResolveProblem(TargetParameters targetParameters)
        {
            if (null == targetParameters)
            {
                throw new ArgumentNullException(nameof(targetParameters));
            }

            double[] inputValues = NetworkDataEncoder.EncodeProblemData(targetParameters);

            double[] output = _network.Compute(inputValues);

            ShootParameters decodedSolution = NetworkDataEncoder.DecodeProblemSolution(new ShootParameters
            {
                Angle = output[0],
                InitialSpeed = output[1]
            });

            return new ProblemDefinition
            {
                Conditions = targetParameters,
                Solution = decodedSolution
            };
        }
    }
}
