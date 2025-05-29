namespace Sefer.Backend.Api.Data.Requests.Curricula;

public class DeleteCurriculumBlockCourseRequest(CurriculumBlockCourse entity)
    : DeleteEntityRequest<CurriculumBlockCourse>(entity);