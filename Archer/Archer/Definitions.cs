namespace Archer
{
    public static class Definitions
    {
        public const double MinAngle = 0.0;

        public static readonly double MaxSolutionsAngle = 45.0;

        public const int MaxPossibleAngle = 90;

        public static readonly int WindSpeedOffset = 10;

        public static readonly int WindSpeedRange = 20;

        public static readonly int MaxWindSpeed = WindSpeedRange - WindSpeedOffset;

        public static readonly int MinWindSpeed = -WindSpeedOffset;

        public static readonly int MinSpeed = 0;

        public static readonly int MaxSpeed = 200;

        public static readonly int AngleRange = (int)(MaxPossibleAngle - MinAngle);

        public static readonly int MinDistance = 50;

        public static readonly int DistanceRange = 450;

        public static readonly int MaxDistance = MinDistance + DistanceRange;

        public const double Margin = 0.0;

        public const double Low = 0.0;// + Margin;

        public const double High = 1.0 - Margin;
    }
}
