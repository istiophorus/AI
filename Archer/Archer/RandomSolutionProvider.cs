using System;

namespace Archer
{
    public sealed class RandomSolutionProvider : IProblemSolver
    {
        private readonly Random _random = new Random(Environment.TickCount);

        public ProblemDefinition ResolveProblem(TargetParameters targetParameters)
        {
            double initialSpeed = _random.Next(100);

            double initialAngle = _random.Next((int)Definitions.MaxSolutionsAngle);

            return new ProblemDefinition
            {
                Conditions = targetParameters,
                Solution = new ShootParameters
                {
                    Angle = initialAngle,
                    InitialSpeed = initialSpeed,
                    Count = 1
                }
            };
        }
    }
}
