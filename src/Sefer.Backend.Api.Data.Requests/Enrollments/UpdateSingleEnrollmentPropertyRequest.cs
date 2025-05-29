namespace Sefer.Backend.Api.Data.Requests.Enrollments;

public class UpdateSingleEnrollmentPropertyRequest(Enrollment entity, string property)
    : UpdateSingleEntityPropertyRequest<Enrollment>(entity, property);