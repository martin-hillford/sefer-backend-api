namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class UpdateCourseHandler(IServiceProvider serviceProvider)
    : UpdateEntityHandler<UpdateCourseRequest, Course>(serviceProvider)
{
    public override async Task<bool> Handle(UpdateCourseRequest request, CancellationToken token)
    {
        // a course can only be updated when it's valid;
        if (await IsValid(request.Entity) == false) return false;

        // check if the course can be found in the database
        var course = await Send(new GetCourseByIdRequest(request.Entity.Id), token);
        if (course == null) { return false; }

        // When in the overall stage is not edit or test, the name, permalink and description are not updated
        var stage = course.OverallStage;

        if (stage is Stages.Edit or Stages.Test)
        {
            course.Name = request.Entity.Name;
            course.Permalink = request.Entity.Permalink;
        }

        // set the fields
        course.Level = request.Entity.Level;
        course.Description = request.Entity.Description;
        course.ShowOnHomepage = request.Entity.ShowOnHomepage;
        course.IsVideoCourse = request.Entity.IsVideoCourse;
        course.Private = request.Entity.Private;
        course.HeaderImage = request.Entity.HeaderImage;
        course.LargeImage = request.Entity.LargeImage;
        course.ThumbnailImage = request.Entity.ThumbnailImage;
        course.Citation = request.Entity.Citation;
        course.IntroductionLink = request.Entity.IntroductionLink;
        course.WebshopLink = request.Entity.WebshopLink;
        course.Author = request.Entity.Author;
        course.Copyright = request.Entity.Copyright;
        course.MaxLessonSubmissionsPerDay = request.Entity.MaxLessonSubmissionsPerDay;
        course.CopyrightLogo = request.Entity.CopyrightLogo;

        // update the object in the database
        var updateRequest = new UpdateCourseRequest(course);
        return await base.Handle(updateRequest, token);
    }
}