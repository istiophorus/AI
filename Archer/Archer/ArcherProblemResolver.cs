using System;

namespace Archer
{
    public sealed class ArcherProblemResolver : IProblemSolver
    {
        private static readonly Random _random = new Random(Environment.TickCount);

        private const double DistanceBuffer = 5.0;

        private readonly IProblemSolver _initialParametersProvider;

        public ArcherProblemResolver(IProblemSolver initialParametersProvider)
        {
            if (null == initialParametersProvider)
            {
                throw new ArgumentNullException(nameof(initialParametersProvider));
            }

            _initialParametersProvider = initialParametersProvider;
        }

        //public static ShootParameters ResolveProblem(TargetParameters targetParameters)
        //{
        //    double initialSpeed = _random.Next(100);

        //    double initialAngle = _random.Next((int)Definitions.MaxAngle);

        //    double distance = _random.Next(400) + 50.0;

        //    double minAngle = Definitions.MinAngle;

        //    double currentAngle = initialAngle;

        //    double maxAngle = Definitions.MaxAngle;

        //    bool canIncreaseSpeed = true;

        //    int counter = 0;

        //    while (true)
        //    {
        //        double maxDistanceForSpeed = ShootCalculator.CalculateMaxDistance(initialSpeed, 45.0);

        //        double timeToGround = ShootCalculator.CalculateTimeToGround(initialSpeed, currentAngle);

        //        counter++;

        //        if ((maxDistanceForSpeed < distance && canIncreaseSpeed))
        //        {
        //            initialSpeed = initialSpeed * 1.2 + 5;

        //            minAngle = Definitions.MinAngle;

        //            maxAngle = Definitions.MaxAngle;

        //            continue;
        //        }

        //        canIncreaseSpeed = false;

        //        double heightAtDistance = ShootCalculator.CalculateHeightAtDistance(initialSpeed, currentAngle, distance);

        //        if (heightAtDistance >= targetParameters.TargetHeight)
        //        {
        //            maxAngle = currentAngle;

        //            currentAngle = (maxAngle + minAngle) / 2;
        //        }
        //        else if (heightAtDistance <= 0.0)
        //        {
        //            minAngle = currentAngle;

        //            currentAngle = (maxAngle + minAngle) / 2;
        //        }
        //        else
        //        {
        //            return new ShootParameters
        //            {
        //                InitialSpeed = initialSpeed,
        //                Angle = currentAngle,
        //                Count = counter
        //            };
        //        }
        //    }
        //}

        public ProblemDefinition ResolveProblem(TargetParameters targetParameters)
        {
            ProblemDefinition problemDefinition = _initialParametersProvider.ResolveProblem(targetParameters);

            double initialSpeed = problemDefinition.Solution.InitialSpeed;

            double initialAngle = problemDefinition.Solution.Angle;

            double currentAngle = initialAngle;

            int counter = 0;

            double maxDistanceForSpeed;

            double minAngle = Definitions.MinAngle;

            double maxAngle = Definitions.MaxAngle;

            do
            {
                initialSpeed = initialSpeed * 1.2 + 5;

                if (targetParameters.WindSpeed > 0)
                {
                    maxDistanceForSpeed = ShootCalculator.CalculateMaxDistance(initialSpeed, 45.0 /* if we can reach this distance at angle 45 degrees then we can leave this speed */);
                }
                else
                {
                    maxDistanceForSpeed = ShootCalculator.CalculateMaxDistance(initialSpeed + targetParameters.WindSpeed, 45.0 /* if we can reach this distance at angle 45 degrees then we can leave this speed */);
                }

                counter++;
            }
            while (maxDistanceForSpeed <= targetParameters.TargetDistance + DistanceBuffer);

            while (counter < 1000)
            {
                ShootParameters adjustedShootParameters = ShootCalculator.AdjustShootParameters(initialSpeed, targetParameters.WindSpeed, currentAngle);

                double heightAtDistance = ShootCalculator.CalculateHeightAtDistance(adjustedShootParameters.InitialSpeed, adjustedShootParameters.Angle, targetParameters.TargetDistance);

                //Console.WriteLine($"{counter} {currentAngle} {maxDistanceForSpeed} {heightAtDistance} {targetParameters.TargetDistance} {initialSpeed} {minAngle} {maxAngle}");

                if (heightAtDistance >= targetParameters.TargetHeight)
                {
                    maxAngle = currentAngle;

                    currentAngle = (maxAngle + minAngle) / 2;
                }
                else if (heightAtDistance <= 0.0)
                {
                    minAngle = currentAngle;

                    currentAngle = (maxAngle + minAngle) / 2;
                }
                else
                {
                    return new ProblemDefinition
                    {
                        Solution = new ShootParameters
                        {
                            InitialSpeed = initialSpeed,
                            Angle = currentAngle,
                            Count = counter
                        },
                        Conditions = targetParameters,
                        IsResolved = true
                    };
                }

                counter++;
            }

            return null;
        }
    }
}
