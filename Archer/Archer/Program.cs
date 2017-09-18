using System;
using System.Collections.Generic;
using System.Linq;

namespace Archer
{
    public static class Program
    {
        private static readonly Random _random = new Random(Environment.TickCount);

        private static ProblemDefinition SingleTargetShootingWithWindTask()
        {
            TargetParameters targetParameters = new TargetParameters();

            targetParameters.TargetHeight = 2.0;

            targetParameters.TargetDistance = _random.Next(450) + 50.0;

            targetParameters.WindSpeed = _random.Next((int)20) - 10.0;

            ProblemDefinition result = ArcherProblemResolver.ResolveProblemAdvanced(targetParameters);

            return result;
        }

        public static void Main()
        {
            const int tasksCount = 1000000;

            Console.BufferHeight = 16000;

            List<ProblemDefinition> problems = new List<ProblemDefinition>(tasksCount);

            for (int q = 0; q < tasksCount; q++)
            {
                ProblemDefinition definition = SingleTargetShootingWithWindTask();

                problems.Add(definition);
            }

            double average = problems.Select(x => x.Solution.Count).Average();
        }
    }
}
