using Sefer.Backend.Api.Data.Requests.Courses;
using Sefer.Backend.Api.Data.Requests.Users;

namespace Sefer.Backend.Api.Data.Algorithms;

/// <summary>
/// The course recommender is a simple algorithm that recommend courses
/// </summary>
public class CourseRecommender(IServiceProvider serviceProvider)
{
    private readonly IMediator _mediator = 
        serviceProvider.GetService<IMediator>() ?? throw new Exception("Cannot create the data context service");

    /// <summary>
    /// This method will recommend a course the given student
    /// </summary>
    // ReSharper disable UnusedMember.Global
    public Task<Course?> GetRecommendation(int studentId) => GetRecommendation(studentId, 12.5f, false);

    /// <summary>
    /// This method will recommend a course the given student
    /// </summary>
    public Task<Course?> GetRecommendation(int studentId, float curriculumPercentage) => GetRecommendation(studentId, curriculumPercentage, false);
    // ReSharper restore UnusedMember.Global

    /// <summary>
    /// This method will recommend a course the given student
    /// </summary>
    /// <param name="studentId">The student to recommend a course for</param>
    /// <param name="includePureRandom"></param>
    /// <returns></returns>
    public Task<Course?> GetRecommendation(int studentId, bool includePureRandom) => GetRecommendation(studentId, 12.5f, includePureRandom);

    /// <summary>
    /// This method will recommend a course the given student
    /// </summary>
    /// <param name="studentId">The student to recommend a course for</param>
    /// <param name="curriculumPercentage"></param>
    /// <param name="includePureRandom"></param>
    private async Task<Course?> GetRecommendation(int studentId, float curriculumPercentage, bool includePureRandom)
    {
        // Check if the student exists
        var student = await _mediator.Send(new GetUserByIdRequest(studentId));
        if (student == null || student.IsMentor) return null;

        // Get all the enrollments and courses the student has completed
        var completedCourses = await GetCompletedCourses(studentId);

        // Check for dyslexia recommendation.
        var spokenCourse = await GetDyslexiaRecommendation(student, completedCourses);
        if (spokenCourse != null) return spokenCourse;

        // If the student has no course completed, send a new student recommendation
        if (!completedCourses.Any()) return await GetNewStudentRecommendation();

        // Try to make a recommendation based on the curricula
        var recommended = await GetCurriculumRecommendation(completedCourses.Keys, curriculumPercentage);
        if (recommended != null) return recommended;

        // Try to make a recommendation based on the levels of the courses
        recommended = await GetLevelBasedRecommendation(studentId);
        if (recommended != null) return recommended;

        // Finally check if a pure random course must be selected
        return includePureRandom ? await GetRandomCourse(studentId) : null;
    }

    /// <summary>
    /// This method will recommend a course the given student with dyslexia preferences
    /// </summary>
    private async Task<Course?> GetDyslexiaRecommendation(User student, Dictionary<int, Course> completedCourses)
    {
        if (!student.PreferSpokenCourses) return null;

        // Dyslexia preference completely overrule any other recommendation methods
        var courses = await _mediator.Send(new GetPublishedCoursesRequest());
        return courses
            .Where(c => c.IsVideoCourse && !completedCourses.ContainsKey(c.Id))
            .OrderBy(c => c.Level)
            .FirstOrDefault();
    }

    /// <summary>
    /// This method will return a recommendation for a new student.
    /// Currently just select a random course from the novice level
    /// </summary>
    private Task<Course?> GetNewStudentRecommendation() => GetRandomCourseInLevel(Levels.Novice);

    /// <summary>
    /// This method will try to make a recommendation based upon the completed course of student and the curricula
    /// </summary>
    /// <param name="completedCourses"></param>
    /// <param name="curriculumPercentage"></param>
    /// <returns></returns>
    private async Task<Course?> GetCurriculumRecommendation(IEnumerable<int> completedCourses, float curriculumPercentage)
    {
        // Get for all the curriculum the courses
        var curricula = await _mediator.Send(new GetCurriculaExtendedRequest());
        var lookup = GetCurriculaLookup(curricula);

        // Get the completion score
        var completion = lookup
            .Select(c => new { CurriculumId = c.Key, Percentage = CompletionPercentage(completedCourses, c.Value) })
            .Where(c => c.Percentage < 100f)
            .OrderByDescending(c => c.Percentage)
            .FirstOrDefault();

        // Depending on the completion determine the action
        if (completion == null) return null;
        if (completion.Percentage == 0f) return null;
        if (completion.Percentage < curriculumPercentage) return null;

        var curriculum = curricula.Single(c => c.Id == completion.CurriculumId);
        return await GetNextCourseForCurriculum(completedCourses, curriculum);
    }

    public async Task<Dictionary<int, float>> GetCurriculumProgress(int studentId)
    {
        var curricula = await _mediator.Send(new GetCurriculaExtendedRequest());
        var lookup = GetCurriculaLookup(curricula);
        return await GetCurriculumProgress(lookup, studentId);
    }

    /// <summary>
    /// This method will calculate the progress a student is making on any of the curricula
    /// </summary>
    /// <param name="curricula"></param>
    /// <param name="studentId">The student to determine the progress for</param>
    /// <returns>a dictionary with the id of the curriculum and the progress percentage</returns>
    private async Task<Dictionary<int, float>> GetCurriculumProgress(Dictionary<int, HashSet<int>> curricula, int studentId)
    {
        // Get all the enrollments and courses the student has completed
        var completedCourses = await GetCompletedCourses(studentId);
        if (!completedCourses.Any()) return curricula.ToDictionary(c => c.Key, _ => 0f);

        // Get the completion score
        var completion = curricula.Select(c => new { CurriculumId = c.Key, Percentage = CompletionPercentage(completedCourses.Keys, c.Value) });

        // and create a lookup table
        return completion.ToDictionary(c => c.CurriculumId, c => c.Percentage);
    }

    /// <summary>
    /// This method will return the course within a curriculum given what the student already has completed
    /// </summary>
    /// <param name="completedCourses">a list of completed courses by the student</param>
    /// <param name="curriculum">the curriculum for which the next course must be selected</param>
    /// <returns>The select course</returns>
    private async Task<Course?> GetNextCourseForCurriculum(IEnumerable<int> completedCourses, Curriculum curriculum)
    {
        if (!curriculum.IsPublished) return null;
        var blocks = curriculum.PublishedCurriculumRevision.Blocks.OrderBy(c => c.SequenceId).ToList();

        var courses = new List<int>();
        blocks.ForEach(block => courses.AddRange(block.BlockCourses.OrderBy(b => b.SequenceId).Select(b => b.CourseId)));

        var course = courses.Except(completedCourses).FirstOrDefault();
        return await _mediator.Send(new GetPublishedCourseByIdRequest(course));
    }

    /// <summary>
    /// This method will determine the completion percentage for each curriculum
    /// </summary>
    /// <param name="completedCourses">a list of completed course by the student</param>
    /// <param name="curriculumCourse">a list of courses with in the curriculum</param>
    /// <returns>A percentage about how much of curriculum was completed by the student</returns>
    private static float CompletionPercentage(IEnumerable<int> completedCourses, IReadOnlyCollection<int> curriculumCourse)
    {
        if (curriculumCourse.Count == 0) return 0f;
        var intersection = curriculumCourse.Intersect(completedCourses);
        return intersection.Count() / (float)curriculumCourse.Count * 100f;
    }

    /// <summary>
    /// This method will return a lookup table for courses within a curriculum
    /// </summary>
    private static Dictionary<int, HashSet<int>> GetCurriculaLookup(List<Curriculum> curricula)
    {
        var dictionary = new Dictionary<int, HashSet<int>>();
        foreach (var curriculum in curricula)
        {
            if (curriculum.PublishedCurriculumRevision == null) continue;
            var courses = curriculum.PublishedCurriculumRevision.Blocks.SelectMany(b => b.BlockCourses).Select(b => b.CourseId).ToHashSet();
            dictionary[curriculum.Id] = courses;
        }
        return dictionary;
    }

    /// <summary>
    /// This method wil return a recommendation based on grade of student on the last enrollment and the levels of the student
    /// </summary>
    private async Task<Course?> GetLevelBasedRecommendation(int studentId)
    {
        var lastEnrollment = await _mediator.Send(new GetLastClosedEnrollmentRequest(studentId));
        if (lastEnrollment == null) return null;

        var nextLevel = lastEnrollment.CourseRevision.Course.Level;
        if (lastEnrollment.Grade is < 0.6f) nextLevel = GetPreviousLevel(nextLevel);
        else if (lastEnrollment.Grade is >= 0.75f) nextLevel = GetNextLevel(nextLevel);

        return await GetRandomCourseInLevel(nextLevel, studentId);
    }

    /// <summary>
    /// This method will return a random course as recommendation
    /// </summary>
    private async Task<Course?> GetRandomCourse(int studentId)
    {
        var completed = await GetCompletedCourses(studentId);
        var courses = await _mediator.Send(new GetPublishedCoursesRequest());
        var available = courses.Where(c => !completed.ContainsKey(c.Id)).ToList();
        return await GetRandomCourse(available, studentId);
    }

    /// <summary>
    /// This method will return all the courses the student has completed
    /// </summary>
    private async Task<Dictionary<int, Course>> GetCompletedCourses(int studentId)
    {
        var request = new GetCompletedCoursesRequest(studentId);
        var enrollments = await _mediator.Send(request);
        return enrollments.Select(e => e.CourseRevision.Course).Distinct().ToDictionary(c => c.Id);
    }

    /// <summary>
    /// This method will determine the next level for a given level
    /// </summary>
    /// <param name="level"></param>
    private static Levels GetNextLevel(Levels level)
    {
        return level switch
        {
            Levels.Novice => Levels.Intermediate,
            Levels.Intermediate => Levels.Advanced,
            Levels.Advanced => Levels.Superior,
            Levels.Superior => Levels.Distinguished,
            Levels.Distinguished => Levels.Distinguished,
            _ => Levels.Undefined
        };
    }

    /// <summary>
    /// This method will determine the next level for a given level
    /// </summary>
    /// <param name="level"></param>
    private static Levels GetPreviousLevel(Levels level)
    {
        return level switch
        {
            Levels.Novice => Levels.Novice,
            Levels.Intermediate => Levels.Novice,
            Levels.Advanced => Levels.Intermediate,
            Levels.Superior => Levels.Advanced,
            Levels.Distinguished => Levels.Superior,
            _ => Levels.Undefined,
        };
    }

    /// <summary>
    /// This method will get a random course in a level.
    /// When a studentId is provided the completed courses of the student will be taken into account
    /// </summary>
    private async Task<Course?> GetRandomCourseInLevel(Levels level, int? studentId = null)
    {
        var courses = await _mediator.Send(new GetPublishedCoursesForLevelRequest(level));
        if (courses.Count == 0) return null;
        if (!studentId.HasValue) return courses.RandomElement();
        return await GetRandomCourse(courses, studentId.Value);
    }

    private async Task<Course?> GetRandomCourse(List<Course> courses, int studentId)
    {
        var completed = await GetCompletedCourses(studentId);
        var available = courses.Where(c => !completed.ContainsKey(c.Id)).AsQueryable();
        return available.Any() ? available.RandomElement() : null;
    }

}