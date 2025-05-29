namespace Sefer.Backend.Api.Data.Handlers.Enrollments;

public class UpdateEnrollmentHandler(IServiceProvider serviceProvider)
    : UpdateEntityHandler<UpdateEnrollmentRequest, Enrollment>(serviceProvider);