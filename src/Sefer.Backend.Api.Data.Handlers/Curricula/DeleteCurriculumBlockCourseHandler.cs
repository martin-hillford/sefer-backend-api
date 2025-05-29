namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class DeleteCurriculumBlockCourseHandler(IServiceProvider serviceProvider)
    : DeleteEntityHandler<DeleteCurriculumBlockCourseRequest, CurriculumBlockCourse>(serviceProvider);