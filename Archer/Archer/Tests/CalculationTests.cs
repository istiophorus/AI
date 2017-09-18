using NUnit.Framework;
using System;

namespace Archer.Tests
{
    [TestFixture]
    public sealed class CalculationTests
    {
        [Test]
        [TestCase(20.0, 90.0, 20.0, 45.0)]
        [TestCase(50.0, 34.0, 0.0, 34.0)]
        public void TestAdjustment(double initialSpeed, double angle, double windSpeed, double expectedAngle)
        {
            ShootParameters result = ShootCalculator.AdjustShootParameters(initialSpeed, windSpeed, angle);

            Assert.AreEqual(expectedAngle, result.Angle);
        }

        private static readonly Random Random = new Random(Environment.TickCount);

        [Test]
        [Repeat(16)]
        public void SolverTest()
        {
            TargetParameters targetParameters = new TargetParameters();

            targetParameters.TargetHeight = 2.0;

            targetParameters.TargetDistance = Random.Next(400);

            targetParameters.WindSpeed = Random.Next(40) - 20;

            ProblemDefinition result = ArcherProblemResolver.ResolveProblemAdvanced(targetParameters);

            Assert.IsNotNull(result);

            ShootParameters adjusted = ShootCalculator.AdjustShootParameters(result.Solution.InitialSpeed, result.Conditions.WindSpeed, result.Solution.Angle);

            double height = ShootCalculator.CalculateHeightAtDistance(adjusted.InitialSpeed, adjusted.Angle, result.Conditions.TargetDistance);

            Assert.LessOrEqual(0.0, height);

            Assert.GreaterOrEqual(targetParameters.TargetHeight, height);
        }
    }
}
