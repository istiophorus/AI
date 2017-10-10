using Accord.Neuro;
using Accord.Neuro.Learning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Archer
{
    public static class Program
    {
        private static readonly Random _random = new Random(Environment.TickCount);

        private static readonly int WindSpeedOffset = 10;

        private static readonly int WindSpeedRange = 20;

        private static readonly int MaxAngle = 90;

        private static readonly int MinAngle = 0;

        private static readonly int AngleRange = MaxAngle - MinAngle;

        private static readonly int MaxWindSpeed = WindSpeedRange - WindSpeedOffset;

        private static readonly int MinWindSpeed = -WindSpeedOffset;

        private static readonly int MinDistance = 50;

        private static readonly int MinSpeed = 0;

        private static readonly int MaxSpeed = 200;

        private static readonly int DistanceRange = 450;

        private static readonly int MaxDistance = MinDistance + DistanceRange;

        private const double Low = 0.0;

        private const double High = 1.0;

        private static double[][] WindSpeedWithOffsetBits = new []
        {
            new []{ Low, Low, Low, Low, Low },
            new []{ Low, Low, Low, Low, High },
            new []{ Low, Low, Low, High, Low },
            new []{ Low, Low, Low, High, High },
            new []{ Low, Low, High, Low, Low },
            new []{ Low, Low, High, Low, High },
            new []{ Low, Low, High, High, Low },
            new []{ Low, Low, High, High, High },
            new []{ Low, High, Low, Low, Low },
            new []{ Low, High, Low, Low, High },
            new []{ Low, High, Low, High, Low },
            new []{ Low, High, Low, High, High },
            new []{ Low, High, High, Low, Low },
            new []{ Low, High, High, Low, High },
            new []{ Low, High, High, High, Low },
            new []{ Low, High, High, High, High },
            new []{ High, Low, Low, Low, Low },
            new []{ High, Low, Low, Low, High },
            new []{ High, Low, Low, High, Low },
            new []{ High, Low, Low, High, High },
            new []{ High, Low, High, Low, Low },
            new []{ High, Low, High, Low, High },
            new []{ High, Low, High, High, Low },
            new []{ High, Low, High, High, High },
            new []{ High, High, Low, Low, Low },
            new []{ High, High, Low, Low, High },
            new []{ High, High, Low, High, Low },
            new []{ High, High, Low, High, High },
            new []{ High, High, High, Low, Low },
            new []{ High, High, High, Low, High },
            new []{ High, High, High, High, Low },
            new []{ High, High, High, High, High }
        };

        private static ProblemDefinition SingleTargetShootingWithWindTask()
        {
            TargetParameters targetParameters = new TargetParameters();

            targetParameters.TargetHeight = 2.0;

            targetParameters.TargetDistance = _random.Next(DistanceRange) + MinDistance;

            targetParameters.WindSpeed = _random.Next(WindSpeedRange) - WindSpeedOffset;

            ProblemDefinition result = ArcherProblemResolver.ResolveProblemAdvanced(targetParameters);

            return result;
        }

        public static void Main()
        {
            List<ProblemDefinition> records = PrepareData();

            LearningData learningData = PrepareLearningData(records.ToArray());

            TrainNetwork(learningData, @"..\Network\network.bin");
        }

        private static double EncodeValue(double value, double minValue, double maxValue)
        {
            return (value - minValue) / (maxValue - minValue) * (High - Low) + Low;
        }

        private static double[] EncodeProblemData(ProblemDefinition input)
        {
            double windSpeedEncoded = EncodeValue(input.Conditions.WindSpeed, MinWindSpeed, MaxWindSpeed);

            double distanceEncoded = EncodeValue(input.Conditions.TargetDistance, MinDistance, MaxDistance);

            return new double[] { windSpeedEncoded, distanceEncoded };
        }

        //private static double[] EncodeProblemData(ProblemDefinition input)
        //{
        //    int windSpeed = (int)input.Conditions.WindSpeed + WindSpeedOffset;

        //    double[] windSpeedValues = WindSpeedWithOffsetBits[windSpeed];

        //    List<double> result = new List<double>();

        //    return result.ToArray();
        //}

        private static double[] EncodeProblemSolution(ProblemDefinition input)
        {
            double angleEncoded = EncodeValue(input.Solution.Angle, MinAngle, MaxAngle);

            double speedEncoded = EncodeValue(input.Solution.InitialSpeed, MinSpeed, MaxSpeed);

            return new double[] { angleEncoded, speedEncoded };
        }

        private static LearningData PrepareLearningData(ProblemDefinition[] inputData)
        {
            Double[][] input = new Double[inputData.Length][];

            Double[][] output = new Double[inputData.Length][];

            Parallel.For(0, inputData.Length, x =>
            {
                input[x] = EncodeProblemData(inputData[x]);
                output[x] = EncodeProblemSolution(inputData[x]);
            });

            return new LearningData
            {
                Input = input,
                Output = output
            };
        }

        private sealed class LearningData
        {
            internal Double[][] Input { get; set; }

            internal Double[][] Output { get; set; }
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

            double maxSpeed = problems.Select(x => x.Solution.InitialSpeed).Max();

            return problems;
        }

        private static readonly Boolean UseBipolar = true;

        private static void TrainNetwork(LearningData learningData, String networkPath)
        {
            ActivationNetwork network = new ActivationNetwork(
                UseBipolar ? (IActivationFunction)new BipolarSigmoidFunction(1) : (IActivationFunction)new SigmoidFunction(),
                2,
                250,
                //40,
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

            Int32 counter = 100000;
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
