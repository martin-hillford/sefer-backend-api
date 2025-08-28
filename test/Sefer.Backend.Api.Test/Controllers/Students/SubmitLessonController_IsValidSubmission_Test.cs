// ReSharper disable InconsistentNaming
using Sefer.Backend.Api.Data.Models.Courses.Lessons;
using Sefer.Backend.Api.Models.Student.Profile;
using BoolQuestion = Sefer.Backend.Api.Data.Models.Courses.Lessons.BoolQuestion;
using Lesson = Sefer.Backend.Api.Data.Models.Courses.Lessons.Lesson;
using MultipleChoiceQuestion = Sefer.Backend.Api.Data.Models.Courses.Lessons.MultipleChoiceQuestion;
using OpenQuestion = Sefer.Backend.Api.Data.Models.Courses.Lessons.OpenQuestion;
using SubmitLessonController = Sefer.Backend.Api.Controllers.Student.SubmitLessonController;

namespace Sefer.Backend.Api.Test.Controllers.Students;

public partial class SubmitLessonControllerTest
{
    [TestMethod]
    public void IsValidSubmission_Null()
    {
        var lesson = new Lesson();
        var isValid = SubmitLessonController.IsValidSubmission(lesson, null);
        Assert.IsFalse(isValid);
    }
    
    [TestMethod]
    public void IsValidSubmission_NotFinal()
    {
        var lesson = new Lesson();
        var submission = new SubmissionPostModel { Final = false };
        var isValid = SubmitLessonController.IsValidSubmission(lesson, submission);
        Assert.IsTrue(isValid);
    }
    
    [TestMethod]
    public void IsValidSubmission_AnswersNull()
    {
        var lesson = new Lesson();
        var submission = new SubmissionPostModel { Final = true };
        var isValid = SubmitLessonController.IsValidSubmission(lesson, submission);
        Assert.IsFalse(isValid);
    }

    [TestMethod]
    public void IsValidSubmission_NoAnswers()
    {
        var boolQuestion = new BoolQuestion { Id = 1, CorrectAnswerIsTrue = true };
        var lesson = new Lesson { Content = [boolQuestion] };
        var submission = new SubmissionPostModel { Final = true };
        var isValid = SubmitLessonController.IsValidSubmission(lesson, submission);
        Assert.IsFalse(isValid);
    }
    
    [TestMethod]
    public void IsValidSubmission_AnswerNotFound()
    {
        var boolQuestion = new BoolQuestion { Id = 1, CorrectAnswerIsTrue = true };
        var lesson = new Lesson { Content = [boolQuestion] };
        var answer = new QuestionAnswerPostModel { Answer = "false", QuestionId = 3 };
        var submission = new SubmissionPostModel { Final = true, Answers = [answer] };
        var isValid = SubmitLessonController.IsValidSubmission(lesson, submission);
        Assert.IsFalse(isValid);
    }
    
    [TestMethod]
    public void IsValidSubmission_QuestionTypeNotMatching()
    {
        var boolQuestion = new BoolQuestion { Id = 3, CorrectAnswerIsTrue = true };
        var lesson = new Lesson { Content = [boolQuestion] };
        var answer = new QuestionAnswerPostModel { Answer = "false", QuestionId = 3, QuestionType = ContentBlockTypes.ElementAudio};
        var submission = new SubmissionPostModel { Final = true, Answers = [answer] };
        var isValid = SubmitLessonController.IsValidSubmission(lesson, submission);
        Assert.IsFalse(isValid);
    }
    
    [TestMethod]
    public void IsValidSubmission_InvalidBooleanAnswer()
    {
        var boolQuestion = new BoolQuestion { Id = 3, CorrectAnswerIsTrue = true };
        var lesson = new Lesson { Content = [boolQuestion] };
        var answer = new QuestionAnswerPostModel { Answer = "NoAnswer", QuestionId = 3, QuestionType = ContentBlockTypes.QuestionBoolean};
        var submission = new SubmissionPostModel { Final = true, Answers = [answer] };
        var isValid = SubmitLessonController.IsValidSubmission(lesson, submission);
        Assert.IsFalse(isValid);
    }
    
    [TestMethod]
    public void IsValidSubmission_ValidBooleanAnswer()
    {
        var boolQuestion = new BoolQuestion { Id = 3, CorrectAnswerIsTrue = true };
        var lesson = new Lesson { Content = [boolQuestion] };
        var answer = new QuestionAnswerPostModel { Answer = "Wrong", QuestionId = 3, QuestionType = ContentBlockTypes.QuestionBoolean};
        var submission = new SubmissionPostModel { Final = true, Answers = [answer] };
        var isValid = SubmitLessonController.IsValidSubmission(lesson, submission);
        Assert.IsTrue(isValid);
    }
    
    [TestMethod]
    public void IsValidSubmission_InvalidOpenAnswer()
    {
        var openQuestion = new OpenQuestion { Id = 3 };
        var lesson = new Lesson { Content = [openQuestion] };
        var answer = new QuestionAnswerPostModel { QuestionId = 3, QuestionType = ContentBlockTypes.QuestionOpen};
        var submission = new SubmissionPostModel { Final = true, Answers = [answer] };
        var isValid = SubmitLessonController.IsValidSubmission(lesson, submission);
        Assert.IsFalse(isValid);
    }
    
    [TestMethod]
    public void IsValidSubmission_ValidOpenAnswer()
    {
        var openQuestion = new OpenQuestion { Id = 3 };
        var lesson = new Lesson { Content = [openQuestion] };
        var answer = new QuestionAnswerPostModel { Answer = "Answer", QuestionId = 3, QuestionType = ContentBlockTypes.QuestionOpen};
        var submission = new SubmissionPostModel { Final = true, Answers = [answer] };
        var isValid = SubmitLessonController.IsValidSubmission(lesson, submission);
        Assert.IsTrue(isValid);
    }
    
    [TestMethod]
    public void IsValidSubmission_InvalidMultipleChoice()
    {
        var lesson = IsValidSubmission_CreateMultipleChoiceQuestionLesson();
        var answer = new QuestionAnswerPostModel { QuestionId = 3, Answer = "19", QuestionType = ContentBlockTypes.QuestionMultipleChoice};
        var submission = new SubmissionPostModel { Final = true, Answers = [answer] };
        var isValid = SubmitLessonController.IsValidSubmission(lesson, submission);
        Assert.IsFalse(isValid);
    }
    
    [TestMethod]
    public void IsValidSubmission_NoAnswerGiven()
    {
        var lesson = IsValidSubmission_CreateMultipleChoiceQuestionLesson();
        var answer = new QuestionAnswerPostModel { QuestionId = 3, Answer = string.Empty, QuestionType = ContentBlockTypes.QuestionMultipleChoice};
        var submission = new SubmissionPostModel { Final = true, Answers = [answer] };
        var isValid = SubmitLessonController.IsValidSubmission(lesson, submission);
        Assert.IsFalse(isValid);
    }
    
    [TestMethod]
    public void IsValidSubmission_AnswerGiven()
    {
        var lesson = IsValidSubmission_CreateMultipleChoiceQuestionLesson();
        var answer = new QuestionAnswerPostModel { QuestionId = 3, Answer = "19,17", QuestionType = ContentBlockTypes.QuestionMultipleChoice};
        var submission = new SubmissionPostModel { Final = true, Answers = [answer] };
        var isValid = SubmitLessonController.IsValidSubmission(lesson, submission);
        Assert.IsFalse(isValid);
    }

    private static Lesson IsValidSubmission_CreateMultipleChoiceQuestionLesson()
    {
        var choiceA = new MultipleChoiceQuestionChoice { Id = 13, QuestionId = 3, IsCorrectAnswer = true };
        var choiceB = new MultipleChoiceQuestionChoice { Id = 17, QuestionId = 3, IsCorrectAnswer = true };
        var multipleChoice = new MultipleChoiceQuestion { Id = 3, Choices = [choiceB, choiceA]};
        return new Lesson { Content = [multipleChoice] };
    }
}