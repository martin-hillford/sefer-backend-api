// ReSharper disable UnusedMember.Global PropertyCanBeMadeInitOnly.Global UnusedAutoPropertyAccessor.Global
using System.Diagnostics.CodeAnalysis;
using Sefer.Backend.Api.Controllers.Student;
using Sefer.Backend.Api.Models.App;
using Sefer.Backend.Api.Shared;
using Sefer.Backend.Api.Views.App;
using EnrollmentView = Sefer.Backend.Api.Views.App.EnrollmentView;

namespace Sefer.Backend.Api.Controllers.App;

/// <summary>
/// This controller deals with syncing with apps that have offline capabilities
/// </summary>
/// <param name="serviceProvider"></param>
/// <remarks>
/// Please obverse for each of the requests made to this controller, every byte counts!
/// App's using these endpoints are supporting offline approaches because internet is slow or limited 
/// </remarks>
[Authorize(Roles = "Student,User")]
[SuppressMessage("ReSharper", "ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator")]
public class SyncController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    /// <summary>
    /// This method takes local enrollments and compare them this the ones present in the database.
    /// Either, the local enrollment is new from the backend and new enrollment is created or local
    /// enrollment is behind the server and the correct information is returned 
    /// </summary>
    /// <remarks>
    /// The sync operates under the strict rule that a client / device always *MUST*
    /// sync before creating a new enrollment. In this way correct order can be enforced
    /// </remarks>
    [HttpPost("/app/sync/push/enrollments")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> PushEnrollments([FromBody] List<LocalEnrollment> localEnrollments)
    {
        // Check if a student is making this request
        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return Forbid();

        // Get all the enrollments of the student known on the server
        var serverEnrollments = await Send(new GetEnrollmentsOfStudentRequest(student.Id));
        var serverLookup = serverEnrollments.GroupBy(e => e.CourseRevision.CourseId).ToDictionary(c => c.Key, c => c.ToList());

        // Now foreach local enrollment it must determine 
        foreach (var local in localEnrollments)
        {
            // Safety check
            if (local.UserId != student.Id) continue;
            
            // When course  could not be found the local is a new enrollment
            if (serverLookup.NotContainsKey(local.CourseId))
            {
                var insertable = local.CreateEnrollment();
                await Send(new AddEnrollmentRequest(insertable));
                local.Id = insertable.Id.ToString();
                continue;
            }
            
            // Given the strict rule, before creating a retake, the device has to be synced.
            // So take always the last created enrollment from the server
            var server = serverLookup[local.CourseId].OrderByDescending(c => c.CreationDate).FirstOrDefault();
            if (server == null) continue;
            
            // We need to detect if the local enrollment is a head of server enrollment
            // Note: the next lesson to follow is on the server determined by the submissions and not stored in the 
            //       database explicitly.
            var isAhead = local.IsCourseCompleted && !server.IsCourseCompleted;
            if (!isAhead) continue;
            
            // If the local enrollment is ahead, update the information.
            // Note: currently trust the grade calculation from the device
            local.Id = server.Id.ToString();
            server.IsCourseCompleted = true;
            server.ClosureDate = DateTime.UtcNow;
            server.Grade = local.Grade;
            server.ModificationDate = DateTime.UtcNow;
            await Send(new UpdateEnrollmentRequest(server));
        }
        
        // Pushing the enrollments is completed. Include in the response the assigned server id
        var view = localEnrollments.Select(enrollment => new PushResult(enrollment)).ToList();
        return Ok(new SyncView<PushResult>(view));
    }

    /// <summary>
    /// This method returns all the enrollments for a given students.
    /// By settings previousSyncTime 
    /// </summary>
    /// <param name="prevSyncTime">The last time the local client was synced with the server</param>
    [HttpGet("/app/sync/pull/enrollments")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> PullEnrollments(long? prevSyncTime = null)
    {
        // Check if a student is making this request
        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return Forbid();

        DateTime? start = prevSyncTime.HasValue
            ? DateTimeOffset.FromUnixTimeSeconds(prevSyncTime.Value).UtcDateTime
            : null;
        
        var enrollments = await Send(new GetEnrollmentsOfStudentRequest(student.Id, null, false, start));
        var view = enrollments.Select(enrollment => new EnrollmentView(enrollment)).ToList();
        return Ok(new SyncView<EnrollmentView>(view)); 
    }
    
    /// <summary>
    /// This method will take the submission from a local device and will process them
    /// The assumptions is that the moment a submission for an enrollment / lesson combination
    /// already exists, the server is leading
    /// </summary>
    /// <param name="localSubmissions"></param>
    /// <returns></returns>
    [HttpPost("/app/sync/push/submissions")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> PushSubmission([FromBody] List<LocalSubmission> localSubmissions)
    {
        // Check if a student is making this request
        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return Forbid();
        
        // Create a lookup for enrollment for performance reasons
        var enrollments = new Dictionary<string, Enrollment>();
        var result = new Dictionary<LessonSubmission, LocalSubmission>();
        
        foreach (var local in localSubmissions)
        {
            // First check if the enrollment exists for this submission
            if (!enrollments.ContainsKey(local.EnrollmentId))
            {
                var enrollment = await Send(new GetEnrollmentByIdRequest(int.Parse(local.EnrollmentId)));
                if (enrollment == null) return BadRequest();
                enrollments.Add(local.EnrollmentId, enrollment);
            }
            
            // The combination of enrollmentId and lessonId must be unique.
            // If the combination does not exist this is submission must be inserted. 
            // Else ignore the submission. it will be fixed when the submission is pulled
            var server = await Send(new SearchSubmissionRequest(int.Parse(local.EnrollmentId), local.LessonId));

            // If there is no server submission, then create on and insert it
            if (server == null)
            {
                // Check if the posted submission is valid. This should also be the case if the app is used!
                var postModel = local.ToPostModel();
                var lesson = await Send(new GetLessonIncludeReferencesRequest(local.LessonId));
                var isValid = SubmitLessonController.IsValidSubmission(lesson, postModel);
                if (!isValid) return BadRequest($"Answers for submission of lesson {local.EnrollmentId} are not valid");
                
                // Now save it to the sever.
                server = local.ToSubmission();
                var answers = local.Answers.Select(a => a.ToQuestionAnswer()).ToList();
                var response = await Send(new SaveSubmissionRequest(server, answers));
                if(!response) return BadRequest($"Could not save answers for submission of lesson {local.EnrollmentId}");
            }
            
            // Add the result to the dictionary
            result.Add(server, local);
        }
        
        var view = result.Select(s => new PushResult(s.Key, s.Value)).ToList();
        return Ok(view);
    }
    
    /// <summary>
    /// This method returns all the submissions for a given students.
    /// By settings previousSyncTime 
    /// </summary>
    /// <param name="prevSyncTime">The last time the local client was synced with the server</param>
    [HttpGet("/app/sync/pull/submissions")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> PullSubmissions (long? prevSyncTime = null)
    {
        // Check if a student is making this request
        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return Forbid();
            
        // Determine the start 
        DateTime? start = prevSyncTime.HasValue
            ? DateTimeOffset.FromUnixTimeSeconds(prevSyncTime.Value).UtcDateTime
            : null;

        // Get all the submissions that should be synced by the device
        var submissions = await Send(new GetSubmissionsByTimeRequest(student.Id, start));
        var view = submissions.Select(LocalSubmission.Create).ToList();
        return Ok(new SyncView<LocalSubmission>(view));
    }
}