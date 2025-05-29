namespace Sefer.Backend.Api.Data.Extensions;

public static class QuestionExtensions
{
    public static bool? IsCorrect(this QuestionAnswer answer, IContentBlock<Lesson> question)
    {
        return answer.QuestionType switch
        {
            ContentBlockTypes.QuestionBoolean => IsCorrectBoolQuestion(answer, question),
            ContentBlockTypes.QuestionMultipleChoice => IsCorrectMultipleChoiceQuestion(answer, question),
            _ => null,
        };
    }

    public static bool IsCorrectBoolQuestion(this QuestionAnswer answer, IContentBlock<Lesson> question)
    {
        var boolQuestion = (BoolQuestion)question;
        var correctBoolAnswer = boolQuestion.CorrectAnswerText;
        var correct = BooleanStringConvertor.Convert(answer.TextAnswer) ==
                      BooleanStringConvertor.Convert(correctBoolAnswer);
        answer.IsCorrectAnswer = correct;
        return correct;
    }

    public static bool IsCorrectMultipleChoiceQuestion(this QuestionAnswer answer, IContentBlock<Lesson> question)
    {
        try
        {
            var mcQuestion = (MultipleChoiceQuestion)question;
            var correctAnswers = mcQuestion.Choices.Where(c => c.IsCorrectAnswer).Select(c => c.Id.ToString()).ToHashSet();
            var givenAnswers = answer.TextAnswer.Split(',');
            answer.IsCorrectAnswer = correctAnswers.Intersect(givenAnswers).Count() == correctAnswers.Count;
        }
        catch (Exception) { answer.IsCorrectAnswer = false; }
        return answer.IsCorrectAnswer == true;
    }
}