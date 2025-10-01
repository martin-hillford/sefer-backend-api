using Sefer.Backend.Api.Models.Admin.Lesson;
using Sefer.Backend.Api.Views.Admin.Lesson;

namespace Sefer.Backend.Api.Controllers.Admin;

[Authorize(Roles = "Admin,CourseMaker")]
public class LessonController(IServiceProvider provider) : BaseController(provider)
{
    private readonly ICryptographyService _cryptographyService = provider.GetService<ICryptographyService>();

    private readonly IFileStorageService _fileStorageService = provider.GetService<IFileStorageService>();

    [HttpGet("/courses/lessons/{lessonId:int}")]
    [ProducesResponseType(typeof(LessonView), 200)]
    public async Task<IActionResult> GetLesson(int lessonId)
    {
        var lesson = await Send(new GetLessonIncludeReferencesRequest(lessonId));
        if (lesson == null) return NotFound();

        var previewQuery = _cryptographyService.ProtectedQueryString("lesson", lessonId.ToString());
        return Json(new LessonView(lesson, previewQuery));
    }

    [AllowAnonymous, HttpGet("/courses/lessons/{lessonId:int}/preview")]
    [ProducesResponseType(typeof(LessonView), 200)]
    public async Task<IActionResult> GetCoursePreview(int lessonId)
    {
        if (!await IsAllowedToLesson(lessonId)) return Unauthorized();
        var lesson = await Send(new GetLessonIncludeReferencesRequest(lessonId));

        var view = new Views.Public.Lessons.LessonView(lesson, _fileStorageService);
        return Json(view);
    }

    [HttpPost("/courses/lessons")]
    [ProducesResponseType(typeof(string), 201)]
    public async Task<ActionResult> InsertLesson([FromBody] LessonPostModel lessonPostModel)
    {
        // First, check and create a valid lesson model from the post
        if (lessonPostModel == null || ModelState.IsValid == false) return BadRequest(ModelState.ValidationState);
        var model = lessonPostModel.ToLesson();
        if (model == null || !await Send(new IsLessonValidRequest(model))) return BadRequest(ModelState.ValidationState);

        // Next, check if the course revision exists and is editable
        var courseRevision = await Send(new GetCourseRevisionByIdRequest(model.CourseRevisionId));
        if (courseRevision is not { IsEditable: true }) return BadRequest();

        // The next step is to validate the content blocks of the lesson
        if (!await IsContentValid(lessonPostModel, 0)) return BadRequest();

        // The next step is to save the full lesson
        var lessonId = await SaveLesson(lessonPostModel, 0);
        if (lessonId == null) return BadRequest();

        // Done processing the lesson! return the created id to the user
        return Json(lessonId);
    }

    [HttpPut("/courses/lessons/{id:int}")]
    [ProducesResponseType(202)]
    public async Task<ActionResult> UpdateLesson([FromBody] LessonPostModel lessonPostModel, int id)
    {
        // First, check and create a valid lesson model from the post
        if (lessonPostModel == null || !ModelState.IsValid) return BadRequest(ModelState.ValidationState);
        if (await Send(new GetLessonByIdRequest(id)) == null) return NotFound();
        var model = lessonPostModel.ToLesson();
        if (model == null || !await Send(new IsLessonValidRequest(model))) return BadRequest(ModelState.ValidationState);

        // Next, check if the course revision exists and is editable
        var courseRevision = await Send(new GetCourseRevisionByIdRequest(model.CourseRevisionId));
        if (courseRevision is not { IsEditable: true }) return BadRequest("Not Editable");

        // The next step is to validate the content blocks of the lesson
        if (!await IsContentValid(lessonPostModel, id)) return BadRequest("Content not valid");

        // The next step is to save the full lesson
        var result = await SaveLesson(lessonPostModel, id);
        return !result.HasValue ? BadRequest("Not Saved") : StatusCode(202);
    }

    [HttpDelete("/courses/lessons/{id:int}")]
    [ProducesResponseType(202)]
    public async Task<ActionResult> DeleteLesson(int id)
    {
        var lesson = await Send(new GetLessonByIdRequest(id));
        if (lesson == null) return NotFound();

        var courseRevision = await Send(new GetCourseRevisionByIdRequest(lesson.CourseRevisionId));
        if (courseRevision == null || courseRevision.IsEditable == false) return BadRequest();

        var deleted = await Send(new DeleteLessonRequest(lesson));
        return !deleted ? StatusCode(500) : StatusCode(202);
    }

    private async Task<bool> IsContentValid(LessonPostModel lessonPostModel, int lessonId)
    {
        // Check each of the content blocks
        foreach (var contentBlock in lessonPostModel.Content)
        {
            var valid = await IsContentBlockValid(contentBlock, lessonId);
            if (!valid) return false;
        }

        // All blocks themselves are valid.
        // A final thing to check is that the sequence numbers are unique
        var sequence = lessonPostModel.Content.Select(c => c.SequenceId).Distinct();
        return sequence.Count() == lessonPostModel.Content.Count;
    }

    private async Task<bool> IsContentBlockValid(ContentBlockPostModel contentBlock, int lessonId)
    {
        switch (contentBlock.Type)
        {
            case ContentBlockTypes.ElementText:
                var textElement = contentBlock.ToTextElement();
                if (!await IsContentBlockValid(lessonId, textElement)) return false;
                break;
            case ContentBlockTypes.ElementImage:
            case ContentBlockTypes.ElementLink:
            case ContentBlockTypes.ElementVideo:
            case ContentBlockTypes.ElementAudio:
            case ContentBlockTypes.ElementYoutube:
            case ContentBlockTypes.ElementVimeo:
                var mediaElement = contentBlock.ToMediaElement();
                if (!await IsContentBlockValid(lessonId, mediaElement)) return false;
                break;
            case ContentBlockTypes.QuestionBoolean:
                var boolQuestion = contentBlock.ToBoolQuestion();
                if (!await IsContentBlockValid(lessonId, boolQuestion)) return false;
                break;
            case ContentBlockTypes.QuestionOpen:
                var openQuestion = contentBlock.ToOpenQuestion();
                if (!await IsContentBlockValid(lessonId, openQuestion)) return false;
                break;
            case ContentBlockTypes.QuestionMultipleChoice:
                var multipleChoiceQuestion = contentBlock.ToMultipleChoiceQuestion();
                if (!await IsMultipleChoiceQuestionValid(lessonId, multipleChoiceQuestion)) return false;
                break;
            default:
                return false;
        }
        return true;
    }

    private async Task<bool> IsContentBlockValid<T>(int lessonId, T contentBlock) where T : class, IContentBlock
    {
        return await Send(new IsContentBlockValidRequest(lessonId, typeof(T), contentBlock));
    }

    private async Task<bool> IsMultipleChoiceQuestionValid(int lessonId, MultipleChoiceQuestion question)
    {
        // deal with the base conditions
        var blockValid = await IsContentBlockValid(lessonId, question);
        if (!blockValid) return false;

        // A multiple choice question should at least two answers
        if (question.Choices == null || question.Choices.Count < 2) return false;

        // Now validate the choices
        var result = await Task.WhenAll(question.Choices.Select(choice => IsMultipleChoiceQuestionChoiceValid(choice, question)));
        if (result.Contains(false)) return false;

        // Now deal with the multiple correct situation
        if (question.IsMultiSelect) return true;
        var count = question.Choices.Where(c => c.IsCorrectAnswer).ToList().Count;
        return count <= 1;
    }

    private async Task<bool> IsMultipleChoiceQuestionChoiceValid(MultipleChoiceQuestionChoice choice, MultipleChoiceQuestion question)
    {
        // if the id of the choice is not 0, this is an update situation
        if (choice.Id > 0)
        {
            var dbChoice = await Send(new GetLessonChoiceByIdRequest(choice.Id));
            if (dbChoice?.QuestionId != question.Id) return false;
            if (question.Id == 0) return false;
        }

        if (!await Send(new IsChoiceValidRequest(choice))) return false;
        var answers = question.Choices.Where(c => c.Answer.Trim().ToLower().Equals(choice.Answer.Trim().ToLower()));
        return answers.Count() <= 1;
    }

    private async Task<int?> SaveLesson(LessonPostModel posted, int lessonId)
    {
        // Check what to do with the lesson: update or save
        var lesson = (lessonId == 0) ? await InsertLessonObject(posted) : await UpdateLessonObject(posted, lessonId);
        if (lesson == null) return null;

        // First, remove the elements that are in the database, but not in the post.
        // Ensure to do this BEFORE the insert, else, because of id references (not existing in the post),
        // the just inserted elements are removed.

        var elements = await Send(new GetLessonContentRequest(lesson));
        foreach (var element in elements)
        {
            switch (element.Type)
            {
                case ContentBlockTypes.ElementText:
                    await DeleteWhenNotExists(posted.Content, element as LessonTextElement);
                    break;
                case ContentBlockTypes.ElementAudio:
                case ContentBlockTypes.ElementImage:
                case ContentBlockTypes.ElementLink:
                case ContentBlockTypes.ElementVideo:
                case ContentBlockTypes.ElementYoutube:
                case ContentBlockTypes.ElementVimeo:
                    await DeleteWhenNotExists(posted.Content, element as MediaElement);
                    break;
                case ContentBlockTypes.QuestionBoolean:
                    await DeleteWhenNotExists(posted.Content, element as BoolQuestion);
                    break;
                case ContentBlockTypes.QuestionOpen:
                    await DeleteWhenNotExists(posted.Content, element as OpenQuestion);
                    break;
                case ContentBlockTypes.QuestionMultipleChoice:
                    if (posted.Content.Any(c => c.Type == element.Type && c.Id == element.Id) == false)
                    {
                        if (element is not MultipleChoiceQuestion question) break;
                        await Send(new DeleteLessonChoiceRequest(question.Choices));
                        await Send(new DeleteMultipleChoiceQuestionRequest(question));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unsupported block type: {element.Type}");
            }
        }

        // A valid lesson is now in the database, now deal with each of the elements
        foreach (var contentBlock in posted.Content)
        {
            switch (contentBlock.Type)
            {
                case ContentBlockTypes.ElementText:
                    var textElement = contentBlock.ToTextElement();
                    var textElementId = await SaveTextElement(lesson, textElement);
                    if (textElementId == null) return null;
                    break;
                case ContentBlockTypes.ElementImage:
                case ContentBlockTypes.ElementLink:
                case ContentBlockTypes.ElementVideo:
                case ContentBlockTypes.ElementAudio:
                case ContentBlockTypes.ElementYoutube:
                case ContentBlockTypes.ElementVimeo:
                    var mediaElement = contentBlock.ToMediaElement();
                    var mediaElementId = await SaveMediaElement(lesson, mediaElement);
                    if (mediaElementId == null) return null;
                    break;
                case ContentBlockTypes.QuestionBoolean:
                    var boolQuestion = contentBlock.ToBoolQuestion();
                    var boolQuestionId = await SaveBoolQuestion(lesson, boolQuestion);
                    if (boolQuestionId == null) return null;
                    break;
                case ContentBlockTypes.QuestionOpen:
                    var openQuestion = contentBlock.ToOpenQuestion();
                    var openQuestionId = await SaveOpenQuestion(lesson, openQuestion);
                    if (openQuestionId == null) return null;
                    break;
                case ContentBlockTypes.QuestionMultipleChoice:
                    var multipleChoiceQuestionId = await SaveMultipleChoiceQuestion(lesson, contentBlock);
                    if (multipleChoiceQuestionId == null) return null;
                    break;
                default:
                    return null;
            }
        }

        // everything is saved, return the id of the lesson that was saved
        return lesson.Id;
    }

    private async Task DeleteWhenNotExists<T>(IEnumerable<ContentBlockPostModel> content, T element)
        where T : class, IContentBlock<Lesson, T>, IContentBlock
    {
        if (content.Any(c => c.Type == element.Type && c.Id == element.Id)) return;
        await Send(new DeleteContentBlockRequest(typeof(T), element.Id));
    }

    private async Task<Lesson> InsertLessonObject(LessonPostModel posted)
    {
        // create the lesson from the posted lesson
        var lesson = posted.ToLesson();

        // We need to deal with sequenceId here (issue SEF-59)
        var revision = await Send(new GetCourseRevisionByIdRequest(lesson.CourseRevisionId));
        if (revision == null) return null;
        var lessons = await Send(new GetLessonsByCourseRevisionRequest(revision.Id));
        var sequenceId = 0;
        if (lessons.Count != 0) sequenceId = Math.Max(lessons.Count, lessons.Max(c => c.SequenceId) + 1);
        lesson.SequenceId = sequenceId;

        // And now we can save the lessons
        return await Send(new AddLessonRequest(lesson)) ? lesson : null;
    }

    private async Task<Lesson> UpdateLessonObject(LessonPostModel posted, int lessonId)
    {
        var lesson = await Send(new GetLessonByIdRequest(lessonId));
        if (lesson == null) return null;

        lesson.Description = posted.Description;
        lesson.ReadBeforeStart = posted.ReadBeforeStart;
        lesson.Name = posted.Name;
        lesson.Number = posted.Number;

        if (!await Send(new UpdateLessonRequest(lesson))) return null;
        return lesson;
    }

    private async Task<int?> SaveTextElement(Lesson lesson, LessonTextElement lessonTextElement)
    {
        if (lessonTextElement.Id > 0)
        {
            var dbElement = await Send(new GetTextElementByIdRequest(lessonTextElement.Id));
            if (dbElement == null || dbElement.LessonId != lesson.Id || dbElement.Type != lessonTextElement.Type) return null;

            dbElement.Content = lessonTextElement.Content;
            dbElement.ForcePageBreak = lessonTextElement.ForcePageBreak;
            dbElement.Number = lessonTextElement.Number;
            dbElement.SequenceId = lessonTextElement.SequenceId;
            dbElement.Heading = lessonTextElement.Heading;
            dbElement.IsMarkDownContent = lessonTextElement.IsMarkDownContent;

            if (!await Send(new UpdateTextElementRequest(dbElement))) return null;
        }
        else
        {
            lessonTextElement.Id = 0;
            lessonTextElement.LessonId = lesson.Id;
            if (!await Send(new AddTextElementRequest(lessonTextElement))) return null;
        }

        return lessonTextElement.Id;
    }

    private async Task<int?> SaveMediaElement(Lesson lesson, MediaElement mediaElement)
    {
        if (mediaElement.Id > 0)
        {
            var dbElement = await Send(new GetMediaElementByIdRequest(mediaElement.Id));
            if (dbElement == null || dbElement.LessonId != lesson.Id || dbElement.Type != mediaElement.Type) return null;

            dbElement.Content = mediaElement.Content;
            dbElement.ForcePageBreak = mediaElement.ForcePageBreak;
            dbElement.Number = mediaElement.Number;
            dbElement.SequenceId = mediaElement.SequenceId;
            dbElement.Heading = mediaElement.Heading;
            dbElement.Url = mediaElement.Url;
            dbElement.IsMarkDownContent = mediaElement.IsMarkDownContent;

            if (!await Send(new UpdateMediaElementRequest(dbElement))) return null;
        }
        else
        {
            mediaElement.Id = 0;
            mediaElement.LessonId = lesson.Id;
            if (!await Send(new AddMediaElementRequest(mediaElement))) return null;
        }

        return mediaElement.Id;
    }

    private async Task<int?> SaveBoolQuestion(Lesson lesson, BoolQuestion question)
    {
        if (question.Id > 0)
        {
            var dbQuestion = await Send(new GetBoolQuestionByIdRequest(question.Id));
            if (dbQuestion == null || dbQuestion.LessonId != lesson.Id || dbQuestion.Type != question.Type) return null;

            dbQuestion.Content = question.Content;
            dbQuestion.ForcePageBreak = question.ForcePageBreak;
            dbQuestion.Number = question.Number;
            dbQuestion.SequenceId = question.SequenceId;
            dbQuestion.CorrectAnswer = question.CorrectAnswer;
            dbQuestion.Heading = question.Heading;
            dbQuestion.IsMarkDownContent = question.IsMarkDownContent;
            dbQuestion.AnswerExplanation = question.AnswerExplanation;

            if (!await Send(new UpdateBoolQuestionRequest(dbQuestion))) return null;
        }
        else
        {
            question.Id = 0;
            question.LessonId = lesson.Id;
            if (!await Send(new AddBoolQuestionRequest(question))) return null;
        }

        return question.Id;
    }

    private async Task<int?> SaveOpenQuestion(Lesson lesson, OpenQuestion question)
    {
        if (question.Id > 0)
        {
            var dbQuestion = await Send(new GetOpenQuestionByIdRequest(question.Id));
            if (dbQuestion == null || dbQuestion.LessonId != lesson.Id || dbQuestion.Type != question.Type) return null;

            dbQuestion.Content = question.Content;
            dbQuestion.ForcePageBreak = question.ForcePageBreak;
            dbQuestion.Number = question.Number;
            dbQuestion.SequenceId = question.SequenceId;
            dbQuestion.Heading = question.Heading;
            dbQuestion.IsMarkDownContent = question.IsMarkDownContent;
            dbQuestion.AnswerExplanation = question.AnswerExplanation;
            dbQuestion.ExactAnswer = question.ExactAnswer;

            if (!await Send(new UpdateOpenQuestionRequest(dbQuestion))) return null;
        }
        else
        {
            question.Id = 0;
            question.LessonId = lesson.Id;
            if (!await Send(new AddOpenQuestionRequest(question))) return null;
        }

        return question.Id;
    }

    private async Task<int?> SaveMultipleChoiceQuestion(Lesson lesson, ContentBlockPostModel contentBlock)
    {
        // saving a multiple-choice question is a bit different: first save the question itself
        var question = contentBlock.ToMultipleChoiceQuestion();

        if (question.Id > 0)
        {
            var dbQuestion = await Send(new GetMultipleChoiceQuestionByIdRequest(question.Id));
            if (dbQuestion == null || dbQuestion.LessonId != lesson.Id || dbQuestion.Type != question.Type) return null;

            dbQuestion.Content = question.Content;
            dbQuestion.ForcePageBreak = question.ForcePageBreak;
            dbQuestion.Number = question.Number;
            dbQuestion.SequenceId = question.SequenceId;
            dbQuestion.IsMultiSelect = question.IsMultiSelect;
            dbQuestion.Heading = question.Heading;
            dbQuestion.IsMarkDownContent = question.IsMarkDownContent;
            dbQuestion.AnswerExplanation = question.AnswerExplanation;

            if (!await Send(new UpdateMultipleChoiceQuestionRequest(dbQuestion))) return null;
        }
        else
        {
            question.Id = 0;
            question.LessonId = lesson.Id;
            if (!await Send(new AddMultipleChoiceQuestionRequest(question))) return null;
        }

        // Then save all the choices with the multiple choice question
        var savedChoiceIds = new HashSet<int>();
        foreach (var choice in contentBlock.Choices)
        {
            var savedChoiceId = await SaveMultipleChoiceQuestionChoice(question, choice.ToChoice());
            if (savedChoiceId == null) return null;
            savedChoiceIds.Add(savedChoiceId.Value);
        }

        // As final set all the choices that are in the database but not have returned should be removed

        var choices = await Send(new GetLessonChoicesByQuestionIdRequest(question.Id));
        foreach (var choice in choices.Where(choice => savedChoiceIds.Contains(choice.Id) == false))
        {
            await Send(new DeleteLessonChoiceRequest(choice));
        }

        // Done processing, return the id of the question
        return question.Id;
    }

    private async Task<int?> SaveMultipleChoiceQuestionChoice(MultipleChoiceQuestion question, MultipleChoiceQuestionChoice choice)
    {
        if (choice.Id > 0)
        {
            var dbChoice = await Send(new GetLessonChoiceByIdRequest(choice.Id));
            if (dbChoice == null || dbChoice.QuestionId != question.Id) return null;

            dbChoice.IsCorrectAnswer = choice.IsCorrectAnswer;
            dbChoice.Answer = choice.Answer;

            if (!await Send(new UpdateLessonChoiceRequest(dbChoice))) return null;
        }
        else
        {
            choice.Id = 0;
            choice.QuestionId = question.Id;
            if (!await Send(new AddLessonChoiceRequest(choice))) return null;
        }

        return choice.Id;
    }

    private async Task<bool> IsAllowedToLesson(int lessonId)
    {
        try
        {
            var user = await GetCurrentUserAsync();
            if (user != null) return user.Role == UserRoles.Admin || user.Role == UserRoles.CourseMaker;

            var data = lessonId.ToString();
            var random = Request.Query["random"];
            var hash = Request.Query["hash"];
            return _cryptographyService.IsProtectedQueryString(data, random, hash);
        }
        catch (Exception) { return false; }
    }
}