﻿using Accord.Neuro;
using Accord.Neuro.Learning;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Archer
{
    public static class Program
    {
        private static readonly Random _random = new Random(Environment.TickCount);

        private const Boolean UseBipolar = true;

        ///private const double Low = (UseBipolar ? -1.0 : 0.0) + Margin;
        ///

        private static TargetParameters PrepareRandomTask()
        {
            TargetParameters targetParameters = new TargetParameters();

            targetParameters.TargetHeight = 2.0;

            lock (_random)
            {
                targetParameters.TargetDistance = _random.Next(Definitions.DistanceRange) + Definitions.MinDistance;

                targetParameters.WindSpeed = _random.Next(Definitions.WindSpeedRange) - Definitions.WindSpeedOffset;
            }

            return targetParameters;
        }

        private static ProblemDefinition SingleTargetShootingWithWindTask()
        {
            TargetParameters targetParameters = PrepareRandomTask();

            ProblemDefinition result = new ArcherProblemResolver(new RandomSolutionProvider()).ResolveProblem(targetParameters);

            return result;
        }

        private static void LearnMode()
        {
            List<ProblemDefinition> records = PrepareData();

            LearningData learningData = PrepareLearningData(records.ToArray());

            TrainNetwork(learningData, @"..\\Networks\\network");
        }

        private static int VerifyProblemsSolutions(ProblemDefinition[] input)
        {
            if (null == input)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (input.Length <= 0)
            {
                throw new ArgumentException(nameof(input));
            }

            int successCounter = 0;

            Parallel.ForEach(input, x =>
            {
                if (VerifySolution(x))
                {
                    Interlocked.Increment(ref successCounter);
                }
            });

            return successCounter;
        }

        private static void TestMode(string networkPath)
        {
            IProblemSolver problemSolver = new NetworkSolutionProvider(networkPath);

            int testsCount = 1000000;

            ConcurrentQueue<TargetParameters> parameters = new ConcurrentQueue<TargetParameters>();

            Parallel.For(0, testsCount, x => parameters.Enqueue(PrepareRandomTask()));

            TargetParameters[] input = parameters.ToArray();

            ConcurrentQueue<ProblemDefinition> problems = new ConcurrentQueue<ProblemDefinition>();

            Parallel.ForEach(input, p => problems.Enqueue(problemSolver.ResolveProblem(p)));

            int successCounter = VerifyProblemsSolutions(problems.ToArray());

            Console.WriteLine($"{testsCount} {successCounter} {successCounter * 1.0 / testsCount}");
        }

        private static bool VerifySolution(ProblemDefinition result)
        {
            ShootParameters adjusted = ShootCalculator.AdjustShootParameters(result.Solution.InitialSpeed, result.Conditions.WindSpeed, result.Solution.Angle);

            double height = ShootCalculator.CalculateHeightAtDistance(adjusted.InitialSpeed, adjusted.Angle, result.Conditions.TargetDistance);

            return height > 0.0 && height <= result.Conditions.TargetHeight;
        }

        public static void Main(string[] args)
        {
            try
            {
                string command = null;

                if (args.Length > 0)
                {
                    command = args[0].ToLowerInvariant();
                }

                switch (command)
                {
                    case "-train":
                    case "train":
                        LearnMode();
                        break;

                    case "-test":
                    case "test":
                        if (args.Length < 2)
                        {
                            throw new ArgumentException("Network bin path has not been provided");
                        }

                        TestMode(args[1]);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static LearningData PrepareLearningData(ProblemDefinition[] inputData)
        {
            Double[][] input = new Double[inputData.Length][];

            Double[][] output = new Double[inputData.Length][];

            Parallel.For(0, inputData.Length, x =>
            {
                input[x] = NetworkDataEncoder.EncodeProblemData(inputData[x].Conditions);
                output[x] = NetworkDataEncoder.EncodeProblemSolution(inputData[x].Solution);
            });

            return new LearningData
            {
                Input = input,
                Output = output
            };
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

            int successCounter = VerifyProblemsSolutions(problems.ToArray());

            Console.WriteLine($"{tasksCount} {successCounter} {successCounter * 1.0 / tasksCount}");

            double average = problems.Select(x => x.Solution.Count).Average();

            double maxSpeed = problems.Select(x => x.Solution.InitialSpeed).Max();

            return problems;
        }

        private static void TrainNetwork(LearningData learningData, String networkPath)
        {
            Dictionary<int, double> resultsMap = new Dictionary<int, double>();

            int nx = 150;

            ActivationNetwork network = new ActivationNetwork(
                UseBipolar ? (IActivationFunction)new BipolarSigmoidFunction(1) : (IActivationFunction)new SigmoidFunction(),
                2,
                nx,
                2);

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

            Int32 counter = 500;
            // loop
            while (counter > 0)
            {
                sw.Reset();

                sw.Start();

                // run epoch of learning procedure
                error = teacher.RunEpoch(learningData.Input, learningData.Output);

                resultsMap[nx] = error;

                sw.Stop();

                //if (error > previousError)
                //{
                //	teacher.LearningRate = teacher.LearningRate * 0.5f;
                //}

                Console.WriteLine(String.Format("{0} {1} {2} {3}", nx, epochIndex, error, sw.Elapsed.TotalSeconds));

                epochIndex++;

                previousError = error;

                counter--;
            }

            network.Save($"{networkPath}_2_{nx}_2_{(int)error}.bin");

            //Double[] output = network.Compute(learningData.Input[0]);			
        }
    }
}
