namespace Sefer.Backend.Api.Data.Handlers.Enrollments;

public class GetEnrollmentByIdHandler(IServiceProvider serviceProvider)
    : GetEntityByIdHandler<GetEnrollmentByIdRequest, Enrollment>(serviceProvider);