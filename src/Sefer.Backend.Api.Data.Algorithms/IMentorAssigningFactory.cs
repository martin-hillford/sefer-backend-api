namespace Sefer.Backend.Api.Data.Algorithms;

public interface IMentorAssigningFactory
{
    IMentorAssigningAlgorithm PrepareAlgorithm(MentorAssigningInput input);
}