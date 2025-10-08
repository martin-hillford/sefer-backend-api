// ReSharper disable ClassNeverInstantiated.Global, CollectionNeverUpdated.Global, UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global, PropertyCanBeMadeInitOnly.Global
using Sefer.Backend.Api.Models.Student.Profile;
// ReSharper disable 

namespace Sefer.Backend.Api.Models.App;

/// <summary>
/// This class represent a local submission being pushed to the server
/// </summary>
public class LocalSubmission
{
    [JsonPropertyName("id")]
    public string Id { get; set;  }
    
    [JsonPropertyName("l_id")]
    public int LocalId { get; set; }
    
    [JsonPropertyName("e_id")]
    public string EnrollmentId { get; set; }
    
    [JsonPropertyName("u_id")]
    public int StudentId { get; set; }
    
    [JsonPropertyName("ls_id")]
    public int LessonId { get; set; }
    
    [JsonPropertyName("s_dt")]
    public long SubmissionDate { get; set; }
    
    [JsonPropertyName("grd")]
    public double? Grade { get; set; }
    
    [JsonPropertyName("ans")]
    public List<LocalAnswer> Answers { get; set; }

    public SubmissionPostModel ToPostModel()
    {
        var answers = Answers.Select(answer => answer.ToPostModel()).ToList();
        return new SubmissionPostModel { EnrollmentId = int.Parse(EnrollmentId), Final = true, Answers = answers};
    }

    public LessonSubmission ToSubmission()
    {
        return new LessonSubmission
        {
            Grade = Grade,
            CreationDate = DateTime.UtcNow,
            ModificationDate = DateTime.UtcNow,
            LessonId = LessonId,
            EnrollmentId = int.Parse(EnrollmentId),
            IsFinal = true,
            ResultsStudentVisible = true,
            SubmissionDate = DateTime.UtcNow,
            Imported = false
        };
    }
    
    public static LocalSubmission Create(LessonSubmission submission)
    {
        return new LocalSubmission
        {
            LessonId = submission.LessonId,
            EnrollmentId = submission.EnrollmentId.ToString(),
            Grade = submission.Grade,
            StudentId = submission.Enrollment.StudentId,
            SubmissionDate = submission.SubmissionDate?.ToUnixTime() ?? 0,
            Id = submission.Id.ToString(),
            Answers = submission.Answers.Select(LocalAnswer.Create).ToList()
        };
    }
}

public class LocalAnswer
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [JsonPropertyName("q_tp")]
    public ContentBlockTypes QuestionType { get; set; }
    
    [JsonPropertyName("q_id")]
    public int QuestionId { get; set; }
    
    [JsonPropertyName("t")]
    public string Text { get; set; }
    
    [JsonPropertyName("c")]
    public List<int> Choices { get; set; }
    
    [JsonPropertyName("b")]
    public bool BoolValue { get; set; }

    public QuestionAnswerPostModel ToPostModel()
    {
        var model = new QuestionAnswerPostModel { QuestionType = QuestionType, QuestionId = QuestionId };

        model.Answer = QuestionType switch
        {
            ContentBlockTypes.QuestionOpen => Text,
            ContentBlockTypes.QuestionBoolean => BoolValue ? "correct" : "incorrect",
            ContentBlockTypes.QuestionMultipleChoice => string.Join(", ", Choices),
            _ => model.Answer
        };

        return model;
    }

    public QuestionAnswer ToQuestionAnswer()
    {
        return new QuestionAnswer()
        {
            QuestionType = QuestionType,
            QuestionId = QuestionId,
            CreationDate = DateTime.UtcNow,
            ModificationDate = DateTime.UtcNow,
            TextAnswer = ToPostModel().Answer
        };
    }
    
    public static LocalAnswer Create(QuestionAnswer answer)
    {
        var local = new LocalAnswer
        {
            QuestionType = answer.QuestionType,
            QuestionId = answer.QuestionId
        };

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (answer.QuestionType)
        {
            case ContentBlockTypes.QuestionOpen:
                local.Text = answer.TextAnswer;
                break;
            case ContentBlockTypes.QuestionBoolean:
                local.BoolValue = answer.BoolAnswer == true;
                break;
            case ContentBlockTypes.QuestionMultipleChoice:
                local.Choices = answer.TextAnswer.Split(',').Select(int.Parse).ToList();
                break;
        }

        return local;
    }
}
