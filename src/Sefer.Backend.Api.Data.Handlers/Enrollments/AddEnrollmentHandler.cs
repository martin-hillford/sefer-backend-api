namespace Sefer.Backend.Api.Data.Handlers.Enrollments;

public class AddEnrollmentHandler(IServiceProvider serviceProvider)
    : AddEntityHandler<AddEnrollmentRequest, Enrollment>(serviceProvider);