namespace Archer
{
    public static class NetworkDataEncoder
    {
        private static double EncodeValue(double value, double minValue, double maxValue)
        {
            return (value - minValue) / (maxValue - minValue) * (Definitions.High - Definitions.Low) + Definitions.Low;
        }

        public static double[] EncodeProblemData(ProblemDefinition input)
        {
            double windSpeedEncoded = EncodeValue(input.Conditions.WindSpeed, Definitions.MinWindSpeed, Definitions.MaxWindSpeed);

            double distanceEncoded = EncodeValue(input.Conditions.TargetDistance, Definitions.MinDistance, Definitions.MaxDistance);

            return new double[] { windSpeedEncoded, distanceEncoded };
        }

        public static double[] EncodeProblemSolution(ProblemDefinition input)
        {
            double angleEncoded = EncodeValue(input.Solution.Angle, Definitions.MinAngle, Definitions.MaxPossibleAngle);

            double speedEncoded = EncodeValue(input.Solution.InitialSpeed, Definitions.MinSpeed, Definitions.MaxSpeed);

            return new double[] { angleEncoded, speedEncoded };
        }
    }
}
