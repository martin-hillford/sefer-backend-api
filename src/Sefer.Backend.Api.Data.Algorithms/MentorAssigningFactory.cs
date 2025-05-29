namespace Sefer.Backend.Api.Data.Algorithms;

public class MentorAssigningFactory : IMentorAssigningFactory
{
    public IMentorAssigningAlgorithm PrepareAlgorithm(MentorAssigningInput input)
    {
        return new MentorAssigning(input);
    }
}