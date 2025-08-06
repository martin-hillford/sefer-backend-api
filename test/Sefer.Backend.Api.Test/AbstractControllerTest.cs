using Microsoft.EntityFrameworkCore;
using Sefer.Backend.Api.Data.Models.Courses.Lessons;
using Sefer.Backend.Api.Models.Student.Profile;

namespace Sefer.Backend.Api.Test;

public abstract class AbstractControllerTest
{
    protected static MockedServiceProvider GetServiceProvider(User currentUser = null)
    {
        var authService = new Mock<IUserAuthenticationService>();
        if(currentUser != null) { authService.Setup(a => a.UserId).Returns(currentUser.Id); }
        var provider = new MockedServiceProvider();
        provider.AddService(authService);
        if (currentUser != null) provider.AddRequestResult<GetUserByIdRequest, User>(currentUser);
        return provider;
    }
    
    protected static SubmissionPostModel CreateSubmissionPostModel(Lesson lesson, IntegrationServices services, bool final)
    {
        var context = services.Provider.GetContext();
        var openQuestions = context.LessonOpenQuestions.Where(l => l.LessonId == lesson.Id).ToList();
        var boolQuestions = context.LessonBoolQuestions.Where(l => l.LessonId == lesson.Id).ToList();
        var mcQuestion = context.LessonMultipleChoiceQuestions
            .Where(l => l.LessonId == lesson.Id)
            .Include(q => q.Choices).
            ToList();
        
        var answers = new List<QuestionAnswerPostModel>();
        answers.AddRange(openQuestions.Select(o => new QuestionAnswerPostModel { Answer = "Answer", QuestionId = o.Id, QuestionType = ContentBlockTypes.QuestionOpen }));
        answers.AddRange(boolQuestions.Select(b => new QuestionAnswerPostModel { Answer = b.CorrectAnswerText, QuestionId = b.Id, QuestionType = ContentBlockTypes.QuestionBoolean }));
        answers.AddRange(mcQuestion.Select(m => new QuestionAnswerPostModel { QuestionId = m.Id, QuestionType = ContentBlockTypes.QuestionMultipleChoice, Answer = string.Join(',', m.Choices.Select(c => c.Id)) }));
        
        return new SubmissionPostModel { Answers = answers, Final = final }; 
    }
}