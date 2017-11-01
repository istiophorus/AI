namespace Archer
{
    public static class NetworkDataEncoder
    {
        private static double EncodeValue(double value, double minValue, double maxValue)
        {
            return (value - minValue) / (maxValue - minValue) * (Definitions.High - Definitions.Low) + Definitions.Low;
        }

        private static double DecodeValue(double value, double minValue, double maxValue)
        {
            return (value - Definitions.Low) / (Definitions.High - Definitions.Low) * (maxValue - minValue) + minValue;
        }

        public static double[] EncodeProblemData(TargetParameters input)
        {
            double windSpeedEncoded = EncodeValue(input.WindSpeed, Definitions.MinWindSpeed, Definitions.MaxWindSpeed);

            double distanceEncoded = EncodeValue(input.TargetDistance, Definitions.MinDistance, Definitions.MaxDistance);

            return new double[] { windSpeedEncoded, distanceEncoded };
        }

        public static double[] EncodeProblemSolution(ShootParameters input)
        {
            double angleEncoded = EncodeValue(input.Angle, Definitions.MinAngle, Definitions.MaxPossibleAngle);

            double speedEncoded = EncodeValue(input.InitialSpeed, Definitions.MinSpeed, Definitions.MaxSpeed);

            return new double[] { angleEncoded, speedEncoded };
        }

        public static ShootParameters DecodeProblemSolution(ShootParameters networkOutput)
        {
            double angleEncoded = DecodeValue(networkOutput.Angle, Definitions.MinAngle, Definitions.MaxPossibleAngle);

            double speedEncoded = DecodeValue(networkOutput.InitialSpeed, Definitions.MinSpeed, Definitions.MaxSpeed);

            return new ShootParameters
            {
                Angle = angleEncoded,
                InitialSpeed = speedEncoded
            };
        }
    }
}
