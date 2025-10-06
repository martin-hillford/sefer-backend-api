// ReSharper disable UnusedMember.Global PropertyCanBeMadeInitOnly.Global UnusedAutoPropertyAccessor.Global
using System.Diagnostics.CodeAnalysis;
using Sefer.Backend.Api.Shared;

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
    public async Task<IActionResult> PushEnrollments(List<LocalEnrollment> localEnrollments)
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
    /// <param name="prevSyncTime"></param>
    /// <returns></returns>
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
}

/// <summary>
/// This 
/// </summary>
/// <param name="data"></param>
/// <typeparam name="T"></typeparam>
public class SyncView<T>(List<T> data)
{
    [JsonPropertyName("s_dt")]
    public long SyncDate = DateTime.UtcNow.ToUnixTime();

    [JsonPropertyName("date")]
    public List<T> Data = data;
}

/// <summary>
/// This is a result from a push.
/// </summary>
/// <param name="local"></param>
public class PushResult(LocalEnrollment local)
{
    public string Id => local.Id;
    
    public int LocalId => local.LocalId;
}

/// <summary>
/// This reprents the local enrollment
/// </summary>
public class LocalEnrollment
{
    [JsonPropertyName("id")]
    public string Id { get; set;  }
    
    [JsonPropertyName("l_id")]
    public int LocalId { get; set; }
    
    [JsonPropertyName("cr_id")]
    public int CourseRevisionId { get; set;  }
    
    [JsonPropertyName("u_id")]
    public int UserId { get; set;  }
    
    [JsonPropertyName("c_id")]
    public int CourseId { get; set;  }
    
    [JsonPropertyName("dn")]
    public bool IsCourseCompleted { get; set;  }
    
    [JsonPropertyName("grd")]
    public double? Grade { get; set;  }
    
    public Enrollment CreateEnrollment()
    {
        return new Enrollment
        {
            CourseRevisionId = CourseRevisionId,
            CreationDate = DateTime.UtcNow,
            ClosureDate = IsCourseCompleted ? DateTime.UtcNow : null,
            Grade = Grade,
            IsCourseCompleted = IsCourseCompleted,
            Imported = false,
            OnPaper = false,
            StudentId = UserId,
            ModificationDate = DateTime.UtcNow
        };
    }
}

/// <summary>
/// An enrollment as send to the client
/// </summary>
/// <param name="enrollment"></param>
public class EnrollmentView(Enrollment enrollment)
{
    [JsonPropertyName("id")]
    public int Id => enrollment.Id;

    /// <summary>
    /// The date the enrollment was created.
    /// </summary>
    [JsonPropertyName("cr_dt")]
    public long CreationDate => enrollment.CreationDate.ToUnixTime();

    /// <summary>
    /// The date the enrollment was modified for the last time
    /// </summary>
    [JsonPropertyName("m_dt")]
    public long? ModificationDate => enrollment.ModificationDate?.ToUnixTime();
    
    /// <summary>
    /// The date the enrollment is closed.
    /// Either because the user ended his enrollment or did complete the course
    /// </summary>
    [JsonPropertyName("cl_dt")]
    public long? ClosureDate => enrollment.ClosureDate?.ToUnixTime();
    
    /// <summary>
    /// The id of the revision of the course that is taken by the student.
    /// </summary>
    [JsonPropertyName("cr_id")]
    public int CourseRevisionId => enrollment.CourseRevisionId;
    
    /// <summary>
    /// The id of the student that is enrolled to the course.
    /// </summary>
    [JsonPropertyName("s_id")]
    public int StudentId => enrollment.StudentId;
    
    /// <summary>
    /// Holds if the User has completed the course. Can be set because the user submitted all the lessons of course to mentor.
    /// Or can be set by an administrator because he knows of completion in the past.
    /// </summary>
    [JsonPropertyName("dn")]
    public int IsCourseCompleted => enrollment.IsCourseCompleted ? 1 : 0;
    
    /// <summary>
    /// Gets the CourseId of this enrollment
    /// </summary>
    [JsonPropertyName("c_id")]
    public int CourseId => enrollment.CourseRevision.CourseId;
    
    /// <summary>
    /// This contains the final grade for of the course (between 0 and zero)
    /// </summary>
    /// <value></value>
    [JsonPropertyName("grd")]
    public double? Grade =>  enrollment.Grade;
}