namespace Sefer.Backend.Api.Data.Algorithms;

/// <summary>
/// Exception thrown when no mentors should be found acting as overflow mentor
/// </summary>
[Serializable]
public class MissingOverflowMentorException : Exception
{
    /// <summary>
    /// Create the exception
    /// </summary>
    public MissingOverflowMentorException() : base("No overflow mentors present") { }
}