using System;

namespace Archer
{
    public static class ArcherProblemResolver
    {
        private const double MinAngle = 0.0;

        private const double MaxAngle = 45.0;

        private static readonly Random _random = new Random(Environment.TickCount);

        private const double DistanceBuffer = 5.0;

        public static ShootParameters ResolveProblem(TargetParameters targetParameters)
        {
            double initialSpeed = _random.Next(100);

            double initialAngle = _random.Next((int)MaxAngle);

            double distance = _random.Next(400) + 50.0;

            double minAngle = MinAngle;

            double currentAngle = initialAngle;

            double maxAngle = MaxAngle;

            bool canIncreaseSpeed = true;

            int counter = 0;

            while (true)
            {
                double maxDistanceForSpeed = ShootCalculator.CalculateMaxDistance(initialSpeed, 45.0);

                double timeToGround = ShootCalculator.CalculateTimeToGround(initialSpeed, currentAngle);

                counter++;

                if ((maxDistanceForSpeed < distance && canIncreaseSpeed))
                {
                    initialSpeed = initialSpeed * 1.2 + 5;

                    minAngle = MinAngle;

                    maxAngle = MaxAngle;

                    continue;
                }

                canIncreaseSpeed = false;

                double heightAtDistance = ShootCalculator.CalculateHeightAtDistance(initialSpeed, currentAngle, distance);

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
                    return new ShootParameters
                    {
                        InitialSpeed = initialSpeed,
                        Angle = currentAngle,
                        Count = counter
                    };
                }
            }
        }

        public static ProblemDefinition ResolveProblemAdvanced(TargetParameters targetParameters)
        {
            double initialSpeed = _random.Next(100);

            double initialAngle = _random.Next((int)MaxAngle);

            double currentAngle = initialAngle;

            int counter = 0;

            double maxDistanceForSpeed;

            double minAngle = MinAngle;

            double maxAngle = MaxAngle;

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
