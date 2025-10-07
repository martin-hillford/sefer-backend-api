namespace Sefer.Backend.Api.Models.App;

/// <summary>
/// This reprints the local enrollment
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