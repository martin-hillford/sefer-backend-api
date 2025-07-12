namespace Sefer.Backend.Api.Controllers.Users;

/// <summary>
/// This controller deals with all lesson audio functionality.
/// Thus, for all the users: public/student/mentors that only will be listening to
/// but also admin users that will be able to upload and change the audio
/// </summary>
/// <param name="provider"></param>
public class LessonAudioController(IServiceProvider provider) : BaseController(provider)
{
    private readonly IAudioStorageService _audioStorageService = provider.GetService<IAudioStorageService>();

    [HttpGet("/lessons/{lessonId:int}/audio")]
    public async Task<IActionResult> GetAudioInformation(int lessonId, [FromQuery] Guid audioReferenceId)
    {
        // Get the lesson and the course revision of the lesson
        var lesson = await Send(new GetLessonByIdRequest(lessonId));
        if (lesson is not { AudioReferenceId: not null }) return NotFound();
        if (lesson.AudioReferenceId != audioReferenceId) return BadRequest();

        var courseRevision = await Send(new GetCourseRevisionByIdRequest(lesson.CourseRevisionId));
        if (courseRevision == null) return NotFound();

        // Check if the user has access to the audio files
        var user = await GetCurrentUserAsync();
        if (courseRevision.Stage != Stages.Published && user?.IsCourseMakerOrAdmin != true) return Unauthorized();

        // Retrieve and return the subtitle file
        var subtitlesFile = await _audioStorageService.RetrieveAudioAsync(lesson.AudioReferenceId.Value);
        if (subtitlesFile == null) return NotFound();
        return Json(subtitlesFile);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("/lessons/audio-upload")]
    [RequestSizeLimit(2147483647)]
    public async Task<IActionResult> UploadAudio(IFormFile audio, int lessonId)
    {
        // Check if the lesson can be found and is open for editing
        var lesson = await Send(new GetLessonByIdRequest(lessonId));
        if (lesson == null) return BadRequest("Lesson not found");
        var oldReferenceId = lesson.AudioReferenceId;

        var courseRevision = await Send(new GetCourseRevisionByIdRequest(lesson.CourseRevisionId));
        if (courseRevision?.Stage != Stages.Edit) return BadRequest("Revision of the lesson is not editable.");

        // Generate a referenceId that will be used throughout the process
        var audioReferenceId = Guid.NewGuid();

        // It is expected that the upload is a zip with mp3 files and a srt file
        // The first step is to unzip the zip file
        var (extracted, outputDirectory) = await SubtitlesFile.CopyAndExtractAsync(audio, audioReferenceId);
        if (!extracted) return BadRequest("Extraction of the audio file failed");

        // Check if this is a valid package with only mp3s and one srt file
        var subtitlesFile = await SubtitlesFile.ReadAudioDirectoryAsync(outputDirectory);
        if (subtitlesFile == null) return BadRequest("Invalid audio package");

        // Now the file is read, it should be uploaded to the audio storage
        var stored = await _audioStorageService.StoreAudioAsync(subtitlesFile, audioReferenceId);
        if (!stored) return BadRequest("Audio package could not be saved into the storage");

        // Update the lesson referenceId to it will become available for the lesson
        var updated = await Send(new SetLessonAudioReferenceRequest(lesson.Id, audioReferenceId));
        if (!updated) return BadRequest("Could not update the audio of the lesson in the database");

        // Deal with reference to old data
        if (!oldReferenceId.HasValue) return Json(subtitlesFile);

        var lessons = await Send(new GetLessonsForAudioReferenceIdRequest(oldReferenceId.Value));
        if (lessons.Count != 0) return Json(subtitlesFile);

        var deleted = await _audioStorageService.DeleteAudioAsync(oldReferenceId.Value);
        if (!deleted) return BadRequest("Could not delete old audio files");
        return Json(subtitlesFile);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("/lessons/{lessonId:int}/audio")]
    public async Task<IActionResult> DeleteAudio(int lessonId)
    {
        var lesson = await Send(new GetLessonByIdRequest(lessonId));
        if (lesson == null || !lesson.AudioReferenceId.HasValue) return NotFound();

        var deleted = await _audioStorageService.DeleteAudioAsync(lesson.AudioReferenceId.Value);
        if (!deleted) return BadRequest("Could not delete old audio files");

        var updated = await Send(new SetLessonAudioReferenceRequest(lesson.Id, null));
        return updated ? NoContent() : BadRequest();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("/lessons/{lessonId:int}/audio/async")]
    public async Task<IActionResult> SyncAudioToPredecessor(int lessonId)
    {
        var lesson = await Send(new GetLessonByIdRequest(lessonId));
        if (lesson == null) return NotFound();

        var synced = await Send(new SyncLessonAudioToPredecessorRequest(lesson.Id));
        return synced ? NoContent() : BadRequest();
    }
}