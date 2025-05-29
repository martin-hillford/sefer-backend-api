namespace Sefer.Backend.Api.Data.Handlers.Validation;

public class TestimonyValidationService(IServiceProvider serviceProvider)
    : CustomValidationService<Testimony>(serviceProvider)
{
    public override async Task<bool> IsValid(Testimony instance)
    {
        if (!await base.IsValid(instance)) return false;

        // Check the reference to student
        if (instance.StudentId == null) return true;
        var student = await Send(new GetUserByIdRequest(instance.StudentId));
        if (student == null || student.IsMentor) return false;

        // courseId valid
        var context = GetDataContext();
        return instance.CourseId == null || context.Courses.Any(c => c.Id == instance.CourseId);

    }
}