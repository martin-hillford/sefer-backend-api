using Token = System.Threading.CancellationToken;

namespace Sefer.Backend.Api.Data.Handlers.CourseRevisions;

public class PublishCourseRevisionHandler(IServiceProvider serviceProvider) : BaseCourseRevisionHandler<PublishCourseRevisionRequest, bool>(serviceProvider)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public override async Task<bool> Handle(PublishCourseRevisionRequest request, Token token)
    {
        // Ensure to load the current published revision of the course that should be closed
        // first check if the revision is publishable
        if (!await Send(new IsPublishableCourseRevisionRequest(request.CourseRevisionId), token)) return false;

        // Get the revision
        var revision = await GetCourseRevision(request.CourseRevisionId);
        if (revision == null) return false;

        return await HandleSync(request);
    }

    private async Task<CourseRevision> GetCourseRevision(int revisionId)
    {
        var context = GetDataContext();
        return await context.CourseRevisions
            .Where(r => r.Id == revisionId)
            .Include(r => r.Course)
            .Include(r => r.Survey)
            .SingleOrDefaultAsync();
    }

    private async Task<bool> HandleSync(PublishCourseRevisionRequest request)
    {
        // Create a context and transaction. Because of the thread safety of context, all subsequent actions
        // should be synchronously!
        var context = GetDataContext();
        var transaction = await context.Database.BeginTransactionAsync();
        var revision = context.CourseRevisions.Single(c => c.Id == request.CourseRevisionId);

        // The first step is to create a new revision
        var previousPublishedRevision = GetPublishedCourseRevision(context, revision.CourseId);
        var newRevision = revision.CreateSuccessor();
        if (!context.Insert(_serviceProvider, newRevision)) return Fail(transaction);

        // Add a safeguard against a failing insert
        try
        {
            // Next create a successor for each lesson
            var lessons = GetLessonsByRevisionId(context, revision.Id);
            foreach (var lesson in lessons)
            {
                var success = CreateLesson(_serviceProvider, context, newRevision, lesson);
                if (!success) return Fail(transaction);
            }

            // Create a successor of the survey
            var survey = context.Surveys.SingleOrDefault(s => s.CourseRevisionId == request.CourseRevisionId);
            if (survey == null)
            {
                var newSurvey = new Survey
                {
                    CourseRevisionId = newRevision.Id,
                    CreationDate = DateTime.UtcNow,
                    EnableCourseRating = true,
                    EnableMentorRating = true,
                    EnableSocialPermissions = true,
                    EnableSurvey = true,
                    EnableTestimonial = true,
                };
                var surveyAdded = context.Insert(_serviceProvider, newSurvey);
                if (!surveyAdded) return Fail(transaction);
            }
            else
            {
                var newSurvey = survey.CreateSuccessor(newRevision);
                var surveyAdded = context.Insert(_serviceProvider, newSurvey);
                if (!surveyAdded) return Fail(transaction);
            }


            // Everything is created now, only update the current revision
            revision.ModificationDate = DateTime.UtcNow;
            revision.Stage = Stages.Published;

            var updated = context.Update(_serviceProvider, revision);
            if (!updated) return Fail(transaction);

            // Deal with any previous revision
            if (previousPublishedRevision != null)
            {
                var closed = CloseCourseRevision(context, previousPublishedRevision.Id);
                if (closed == false) return Fail(transaction);
            }

            // Everything done
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            try { await transaction.RollbackAsync(); } catch { return false; }
            try { await Send(new RemoveCourseRevisionRequest(newRevision.Id)); } catch { return false; }
            return false;
        }

        return true;
    }

    private bool CreateLesson(IServiceProvider serviceProvider, DataContext context, CourseRevision newRevision, Lesson lesson)
    {
        // First check if the revision and the lessons are valid
        if (lesson == null || IsValidEntity(lesson) == false) return false;
        if (lesson.CourseRevisionId != newRevision.PredecessorId) return false;

        // Next create a new lesson
        var newLesson = lesson.CreateSuccessor(newRevision);
        if (!context.Insert(serviceProvider, newLesson)) return false;

        // Now comes the 'nasty' bit for each element and question a successor must be created
        var success =
            CreateBlock<LessonTextElement>(serviceProvider, context, lesson, newLesson) &&
            CreateBlock<BoolQuestion>(serviceProvider, context, lesson, newLesson) &&
            CreateBlock<OpenQuestion>(serviceProvider, context, lesson, newLesson) &&
            CreateBlock<MediaElement>(serviceProvider, context, lesson, newLesson) &&
            CreateMultiQuestion(serviceProvider, context, lesson, newLesson);

        // If the creation was not a success, delete the lesson
        return success;
    }

    private bool CreateBlock<T>(IServiceProvider serviceProvider, DataContext context, Lesson lesson, Lesson successorLesson)
        where T : class, IContentBlock<Lesson, T>
        => ContentBlockHandlerMethods.CreateSuccessor<T>(serviceProvider, context, lesson, successorLesson);

    private bool CreateMultiQuestion(IServiceProvider serviceProvider, DataContext context, IEntity lesson, Lesson successorLesson)
    {
        var questions = GetQuestions(context, lesson);
        foreach (var question in questions)
        {
            var successor = question.CreateSuccessor(successorLesson);
            if (!context.Insert(serviceProvider, successor)) return false;

            // Also create the choices for each question
            context.Entry(question).Collection(q => q.Choices).Load();
            var choices = question.Choices.ToList().Select(choice => choice.CreateSuccessor(successor));
            foreach (var successorChoice in choices)
            {
                var added = context.Insert(serviceProvider, successorChoice);
                if (added == false) { return false; }
            }
        }
        return true;
    }

    private List<MultipleChoiceQuestion> GetQuestions(DataContext context, IEntity lesson)
    {
        return context.LessonMultipleChoiceQuestions
            .Where(q => q.LessonId == lesson.Id)
            .Include(q => q.Choices)
            .OrderBy(q => q.SequenceId)
            .ToList();
    }
}