using Accord.Neuro;
using Accord.Neuro.Learning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            List<ProblemDefinition> learningData = PrepareData();
        }

        private static List<ProblemDefinition> PrepareData()
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

            return problems;
        }

        private static readonly Boolean UseBipolar = true;

        private static void TrainNetwork(LearningData learningData, String networkPath)
        {
            ActivationNetwork network = new ActivationNetwork(
                UseBipolar ? (IActivationFunction)new BipolarSigmoidFunction(1) : (IActivationFunction)new SigmoidFunction(),
                784,
                784,
                10);

            network.Randomize();

            Int32 epochIndex = 0;

            new NguyenWidrow(network).Randomize();

            //// create teacher
            //PerceptronLearning teacher = new PerceptronLearning(network);// new BackPropagationLearning(network);
            //PerceptronLearning teacher = new PerceptronLearning(network);// new BackPropagationLearning(network);
            ParallelResilientBackpropagationLearning teacher = new ParallelResilientBackpropagationLearning(network);

            //teacher.LearningRate = 0.0125;
            ////teacher.Momentum = 0.5f;

            Double error = Double.MaxValue;

            Double previousError = Double.MaxValue;

            Stopwatch sw = new Stopwatch();

            Int32 counter = 100;
            // loop
            while (counter > 0)
            {
                sw.Reset();

                sw.Start();

                // run epoch of learning procedure
                error = teacher.RunEpoch(learningData.Input, learningData.Output);

                sw.Stop();

                //if (error > previousError)
                //{
                //	teacher.LearningRate = teacher.LearningRate * 0.5f;
                //}

                Console.WriteLine(String.Format("{0} {1} {2}", epochIndex, error, sw.Elapsed.TotalSeconds));

                epochIndex++;

                previousError = error;

                counter--;
            }

            network.Save(networkPath);

            //Double[] output = network.Compute(learningData.Input[0]);			
        }
    }
}
