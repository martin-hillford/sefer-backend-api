namespace Sefer.Backend.Api.Data.Algorithms;

public class CurriculumProgressCalculator(IServiceProvider serviceProvider)
{
    public async Task<Dictionary<int, float>> GetCurriculumProgress(int studentId)
    {
        // Just use the internal method of the course recommender for this, since the recommender had
        // everything that is needed for these calculations.
        var recommender = new CourseRecommender(serviceProvider);
        return await recommender.GetCurriculumProgress(studentId);
    }
}