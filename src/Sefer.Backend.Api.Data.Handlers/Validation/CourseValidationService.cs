namespace Sefer.Backend.Api.Data.Handlers.Validation;

public class CourseValidationService(IServiceProvider serviceProvider)
    : CustomValidationService<Course>(serviceProvider)
{
    public override async Task<bool> IsValid(Course instance)
    {
        if (!await base.IsValid(instance)) return false;

        return
            await IsCourseNameUnique(instance.Id, instance.Name) &&
            await IsCoursePermalinkUnique(instance.Id, instance.Permalink);
    }

    private async Task<bool> IsCoursePermalinkUnique(int? courseId, string permalink)
    {
        var course = await Send(new GetCourseByPermalinkRequest(permalink));
        return course == null || course.Id == courseId;
    }

    private async Task<bool> IsCourseNameUnique(int? courseId, string name)
        => await Send(new IsCourseNameUniqueRequest(courseId, name));
}