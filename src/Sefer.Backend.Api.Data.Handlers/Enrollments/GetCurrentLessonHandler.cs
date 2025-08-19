namespace Sefer.Backend.Api.Data.Handlers.Enrollments;

public class GetCurrentLessonHandler(IServiceProvider serviceProvider)
    : Handler<GetCurrentLessonRequest, (Lesson, LessonSubmission, Enrollment)>(serviceProvider)
{
    public override async Task<(Lesson, LessonSubmission, Enrollment)> Handle(GetCurrentLessonRequest request, CancellationToken token)
    {
        // Get the current enrollment of the student
        var student = await Send(new GetUserByIdRequest(request.StudentId), token);
        if (student?.IsStudent != true) return default;
        
        // Note: while enrollments are plural, they are filtered using the enrollmentId
        var enrollments = await Send(new GetActiveEnrollmentsOfStudentRequest(student.Id), token);
        var enrollment = enrollments.FirstOrDefault(enrollment => enrollment.Id == request.EnrollmentId);
        if (enrollment == null) return default;

        // Get the last submission of the student in that enrollment
        var context = GetDataContext();
        var lastSubmission = await context.LessonSubmissions
            .Where(s => s.EnrollmentId == enrollment.Id)
            .OrderByDescending(s => s.Lesson.SequenceId)
            .FirstOrDefaultAsync(token);

        // There are three situations:
        // 1) The student has not yet submitted a lesson in this enrollment
        if (lastSubmission == null)
        {
            return Return(enrollment, (null, await UnSubmitted(context, enrollment, token)));
        }

        // 2) The student has saved a draft submission
        if (!lastSubmission.IsFinal && !lastSubmission.Imported)
        {
            return Return(enrollment, (lastSubmission, await DraftSubmission(context, lastSubmission, token)));
        }

        // 3) The student has submitted the previous lesson and is ready for the next
        return Return(enrollment, (null, await NextLesson(context, lastSubmission, enrollment, token)));
    }

    private static (Lesson, LessonSubmission, Enrollment) Return(Enrollment enrollment, (LessonSubmission, Lesson) tuple)
    {
        var (lessonSubmission, lesson) = tuple;
        return (lesson, lessonSubmission, enrollment);
    }

    private async Task<Lesson> UnSubmitted(DataContext context, Enrollment enrollment, CancellationToken token)
    {
        var publishedCourseRevision = enrollment.CourseRevisionId;
        var lessonId = (await context.Lessons
            .Where(l => l.CourseRevisionId == publishedCourseRevision)
            .OrderBy(l => l.SequenceId)
            .FirstOrDefaultAsync(token))?.Id;

        return await GetLesson(lessonId, token);
    }

    private async Task<Lesson> DraftSubmission(DataContext context, LessonSubmission lastSubmission, CancellationToken token)
    {
        context.Collection(lastSubmission, l => l.Answers);
        return await GetLesson(lastSubmission.LessonId, token);
    }

    private async Task<Lesson> NextLesson(DataContext context, LessonSubmission lastSubmission, Enrollment enrollment, CancellationToken token)
    {
        // Get the last lessons that the student submitted
        var publishedCourseRevision = enrollment.CourseRevisionId;
        var lastLesson = await context.Lessons.SingleOrDefaultAsync(l => l.Id == lastSubmission.LessonId, token);
        if (lastLesson == null) return null;

        // And now find the next lesson (the one after the lastLessons)
        var lessonId = (await context.Lessons
            .Where(l => l.CourseRevisionId == publishedCourseRevision && l.SequenceId > lastLesson.SequenceId)
            .OrderBy(l => l.SequenceId)
            .FirstOrDefaultAsync(token))?.Id;

        return await GetLesson(lessonId, token);
    }

    private async Task<Lesson> GetLesson(int? lessonId, CancellationToken token)
    {
        if (lessonId == null) return null;
        var lesson = await Send(new GetLessonIncludeReferencesRequest(lessonId), token);
        return lesson;
    }
}