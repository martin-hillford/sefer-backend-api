namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class AddCourseHandler(IServiceProvider serviceProvider) : Handler<AddCourseRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(AddCourseRequest request, CancellationToken token)
    {
        if (request.Entity == null || request.Entity.Id > 0) return false;
        if (!await IsValidAsync(request.Entity)) return false;

        var context = GetDataContext();
        await using var transaction = context.BeginTransaction();

        // First check if the instance is valid
        try
        {
            // Set some insert fields
            request.Entity.Id = 0;
            request.Entity.CreationDate = DateTime.UtcNow;
            request.Entity.ModificationDate = null;

            // Insert the course to the database
            context.Courses.Add(request.Entity);
            await context.SaveChangesAsync(token);

            // Create a new revision for the course
            var revision = new CourseRevision
            {
                AllowSelfStudy = false,
                Version = 1,
                Course = request.Entity,
                Stage = Stages.Edit,
                CourseId = request.Entity.Id,
                CreationDate = DateTime.UtcNow
            };

            // insert it in the database
            context.CourseRevisions.Add(revision);
            await context.SaveChangesAsync(token);

            // Create a new survey for the course revision
            var survey = new Survey
            {
                CourseRevisionId = revision.Id,
                CreationDate = DateTime.UtcNow,
                EnableCourseRating = true,
                EnableMentorRating = true,
                EnableSocialPermissions = true,
                EnableSurvey = true,
                EnableTestimonial = true,
            };
            context.Surveys.Add(survey);
            await context.SaveChangesAsync(token);

            // and return that the save was a success
            await transaction.CommitAsync(token);
            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(token);
            return false;
        }
    }
}