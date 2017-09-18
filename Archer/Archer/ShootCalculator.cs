using System;

namespace Archer
{
    public static class ShootCalculator
    {
        private const double G = 9.81;

        private const double G2 = 2 * G;

        public static double Round(this double input, int decimals)
        {
            return Math.Round(input, decimals);
        }

        public static ShootParameters AdjustShootParameters(double initialSpeed, double windSpeed, double currentAngle)
        {
            double vertical = initialSpeed * Math.Sin(currentAngle.AngleAsRadians());

            double horizontal = initialSpeed * Math.Cos(currentAngle.AngleAsRadians()) + windSpeed;

            ShootParameters result = new ShootParameters
            {
                InitialSpeed = Math.Sqrt(vertical * vertical + horizontal * horizontal).Round(6),
                Angle = Math.Atan(vertical / horizontal).RadiansAsAngle().Round(6)
            };

            return result;
        }

        public static double CalculateTimeToGround(double initialSpeed, double angleDegrees)
        {
            return 2 * initialSpeed * Math.Sin(angleDegrees.AngleAsRadians()) / G;
        }

        public static double CalculateMaxDistance(double initialSpeed, double angleDegrees)
        {
            double angleRadians = angleDegrees.AngleAsRadians();

            return initialSpeed * initialSpeed * Math.Sin(2 * angleRadians) / G;
        }

        public static double RadiansAsAngle(this double input)
        {
            return input * 180.0 / Math.PI;
        }

        public static double AngleAsRadians(this double input)
        {
            return input * Math.PI / 180.0;
        }

        public static double CalculateHeightAtDistance(double initialSpeed, double angleDegrees, double distance)
        {
            double angleRadians = angleDegrees.AngleAsRadians();

            double time = distance / (initialSpeed * Math.Cos(angleRadians));

            double heightAtPosition = initialSpeed * Math.Sin(angleRadians) * time - G * time * time / 2;

            return heightAtPosition;
        }

        public static bool VerifyShootParameters(double initialSpeed, double windSpeed, double angleDegrees, double targetDistance, double targetHeight)
        {
            double heightAtDistance = CalculateHeightAtDistance(initialSpeed, angleDegrees, targetDistance);

            if (heightAtDistance <= 0)
            {
                return false;
            }

            if (heightAtDistance >= targetHeight)
            {
                return false;
            }

            return true;
        }
    }
}

