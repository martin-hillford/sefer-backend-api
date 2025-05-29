namespace Sefer.Backend.Api.Data.Handlers.CourseRevisions;

public class GetQuestionsOfCourseRevisionHandler(IServiceProvider serviceProvider)
    : Handler<GetQuestionsOfCourseRevisionRequest, List<(Lesson Lesson, List<Question> Questions)>>(serviceProvider)
{
    public override async Task<List<(Lesson Lesson, List<Question> Questions)>> Handle(GetQuestionsOfCourseRevisionRequest request, CancellationToken token)
    {
        // no revision, no lessons
        var revision = await Send(new GetCourseRevisionByIdRequest(request.CourseRevisionId), token);
        if (revision == null) return [];

        // first step, load the lessons of the revision is the correct order
        var context = GetDataContext();
        var lessons = await context.Lessons
            .Where(l => l.CourseRevisionId == revision.Id)
            .OrderBy(l => l.SequenceId)
            .ToListAsync(token);

        // next step, load all the different question, they are ordered in the right manner
        var boolQuestions = await context.LessonBoolQuestions
            .Where(q => q.Lesson.CourseRevisionId == revision.Id)
            .Include(q => q.Lesson)
            .OrderBy(q => q.Lesson.SequenceId).ThenBy(q => q.SequenceId)
            .ToListAsync(token);

        var openQuestions = await context.LessonOpenQuestions
            .Where(q => q.Lesson.CourseRevisionId == revision.Id)
            .Include(q => q.Lesson)
            .OrderBy(q => q.Lesson.SequenceId).ThenBy(q => q.SequenceId)
            .ToListAsync(token);

        var multipleChoiceQuestions = await context.LessonMultipleChoiceQuestions
            .Where(q => q.Lesson.CourseRevisionId == revision.Id)
            .Include(q => q.Lesson).Include(q => q.Choices)
            .OrderBy(q => q.Lesson.SequenceId).ThenBy(q => q.SequenceId)
            .ToListAsync(token);

        // Dispose the context
        await context.DisposeAsync();

        // Now a structure should be build
        var structure = new Dictionary<Lesson, Dictionary<int, Question>>();
        boolQuestions.ForEach(q => AddLessonToQuestionStructure(q, q.Lesson, structure));
        openQuestions.ForEach(q => AddLessonToQuestionStructure(q, q.Lesson, structure));
        multipleChoiceQuestions.ForEach(q => AddLessonToQuestionStructure(q, q.Lesson, structure));

        // Now create the result structure
        var results = new List<(Lesson Lesson, List<Question> Questions)>();
        foreach (var lesson in lessons)
        {
            var questions = new List<Question>();
            if (structure.TryGetValue(lesson, out var value)) questions.AddRange(value.OrderBy(e => e.Key).Select(e => e.Value));
            results.Add((lesson, questions));
        }

        // Now the whole sorting and loading is done, return the result
        return results;
    }

    private static void AddLessonToQuestionStructure(Question question, Lesson lesson, Dictionary<Lesson, Dictionary<int, Question>> structure)
    {
        if (structure.ContainsKey(lesson) == false) structure.Add(lesson, new Dictionary<int, Question>());
        structure[lesson].Add(question.SequenceId, question);
    }
}