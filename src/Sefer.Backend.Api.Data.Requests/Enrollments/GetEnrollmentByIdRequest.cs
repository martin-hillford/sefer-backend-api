namespace Sefer.Backend.Api.Data.Requests.Enrollments;

public class GetEnrollmentByIdRequest(int? id) : GetEntityByIdRequest<Enrollment>(id);