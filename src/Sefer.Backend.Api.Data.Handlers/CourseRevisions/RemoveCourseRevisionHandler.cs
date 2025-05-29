namespace Sefer.Backend.Api.Data.Handlers.CourseRevisions;

public class RemoveCourseRevisionHandler(IServiceProvider provider) : Handler<RemoveCourseRevisionRequest, bool>(provider)
{
    public override async Task<bool> Handle(RemoveCourseRevisionRequest request, CancellationToken token)
    {
        try
        {
            // N.B. The system is not designed with the concept of course revisions
            // being removed. However, because of some bugs and issues with the transaction (lack of)
            // this method should be able to remove the course revision.
            var context = GetDataContext();

            // Check if the revision exists and is editable
            var revision = await context.CourseRevisions.SingleOrDefaultAsync(c => c.Id == request.CourseRevisionId, token);
            if (revision is not { IsEditable: true }) return false;

            // Check if the revision has no revision being depends on it
            var dependencies = await context.CourseRevisions.AnyAsync(c => c.PredecessorId == request.CourseRevisionId, token);
            if (dependencies) return false;

            // Get all the lessons of this revision
            var lessons = await context.Lessons.Where(l => l.CourseRevisionId == revision.Id).ToListAsync(token);

            // Remove the content of each lesson
            foreach (var lesson in lessons)
            {
                // First get and remove any multiple choice answer related to the lesson
                var multipleChoiceOptions = await context.LessonMultipleChoiceQuestionChoices.Where(c => c.Question.LessonId == lesson.Id).ToListAsync(token);
                context.LessonMultipleChoiceQuestionChoices.RemoveRange(multipleChoiceOptions);
                await context.SaveChangesAsync(token);

                // Next get and remove all the different type of questions and elements
                var boolQuestions = await context.LessonBoolQuestions.Where(l => l.LessonId == lesson.Id).ToListAsync(token);
                var multipleQuestions = await context.LessonMultipleChoiceQuestions.Where(l => l.LessonId == lesson.Id).ToListAsync(token);
                var openQuestions = await context.LessonOpenQuestions.Where(l => l.LessonId == lesson.Id).ToListAsync(token);
                var mediaElements = await context.LessonMediaElements.Where(l => l.LessonId == lesson.Id).ToListAsync(token);
                var textElements = await context.LessonTextElements.Where(l => l.LessonId == lesson.Id).ToListAsync(token);

                context.LessonBoolQuestions.RemoveRange(boolQuestions);
                context.LessonMultipleChoiceQuestions.RemoveRange(multipleQuestions);
                context.LessonOpenQuestions.RemoveRange(openQuestions);
                context.LessonMediaElements.RemoveRange(mediaElements);
                context.LessonTextElements.RemoveRange(textElements);
                await context.SaveChangesAsync(token);
            }

            // ALso deal with the survey
            var survey = await context.Surveys.SingleOrDefaultAsync(s => s.CourseRevisionId == revision.Id, token);
            if (survey != null)
            {
                context.Surveys.RemoveRange(survey);
                await context.SaveChangesAsync(token);
            }

            // Finally remove the lessons and revision itself
            context.Lessons.RemoveRange(lessons);
            await context.SaveChangesAsync(token);

            context.CourseRevisions.Remove(revision);
            await context.SaveChangesAsync(token);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}