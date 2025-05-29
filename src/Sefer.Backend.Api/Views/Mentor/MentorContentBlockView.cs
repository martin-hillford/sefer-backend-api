// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
using Sefer.Backend.Api.Views.Public.Lessons;

namespace Sefer.Backend.Api.Views.Mentor;

public class MentorContentBlockView : ContentBlockView
{
    public readonly string CorrectAnswerText;


    public MentorContentBlockView(LessonTextElement element) : base(element) { }

    public MentorContentBlockView(MediaElement element) : base(element) { }

    public MentorContentBlockView(OpenQuestion question) : base(question) { }

    public MentorContentBlockView(BoolQuestion question) : base(question)
    {
        CorrectAnswerText = question.CorrectAnswerText;
    }

    public MentorContentBlockView(MultipleChoiceQuestion question) : base(question)
    {
        var correct = question.Choices.Where(c => c.IsCorrectAnswer).Select(c => c.Id);
        CorrectAnswerText = string.Join(',', correct);
    }
}