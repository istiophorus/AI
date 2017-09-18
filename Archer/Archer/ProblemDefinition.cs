namespace Archer
{
    public sealed class ProblemDefinition
    {
        public bool IsResolved { get; set; }

        public TargetParameters Conditions { get; set; }

        public ShootParameters Solution { get; set; }
    }
}
