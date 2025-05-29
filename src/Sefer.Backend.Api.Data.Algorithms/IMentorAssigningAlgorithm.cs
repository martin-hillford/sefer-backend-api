namespace Sefer.Backend.Api.Data.Algorithms;

/// <summary>
/// Interface to defining mentor assigning algorithms
/// </summary>
public interface IMentorAssigningAlgorithm
{
    /// <summary>
    /// This method return the assigned mentor
    /// </summary>
    /// <returns></returns>
    User GetMentor();
}