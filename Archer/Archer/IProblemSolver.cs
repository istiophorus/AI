namespace Archer
{
    public interface IProblemSolver
    {
        ProblemDefinition ResolveProblem(TargetParameters targetParameters);
    }
}
