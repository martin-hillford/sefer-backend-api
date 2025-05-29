namespace Sefer.Backend.Api.Data.Handlers.Enrollments;

public class UpdateSingleEnrollmentPropertyHandler(IServiceProvider serviceProvider)
    : UpdateSingleEntityPropertyHandler<UpdateSingleEnrollmentPropertyRequest, Enrollment>(serviceProvider);