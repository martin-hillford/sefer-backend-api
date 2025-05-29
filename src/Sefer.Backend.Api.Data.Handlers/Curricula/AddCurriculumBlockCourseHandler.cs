namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class AddCurriculumBlockCourseHandler(IServiceProvider serviceProvider)
    : AddEntityHandler<AddCurriculumBlockCourseRequest, CurriculumBlockCourse>(serviceProvider);