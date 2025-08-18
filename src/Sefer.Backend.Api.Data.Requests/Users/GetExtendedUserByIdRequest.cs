namespace Sefer.Backend.Api.Data.Requests.Users;

public class GetExtendedUserByIdRequest(int userId) : IRequest<ExtendedUser>
{
    public readonly int UserId = userId;
}

public class ExtendedUser
{
    public User User { get; set; }
    
    public UserLastActivity LastActivity { get; set; }
    
    public bool IsActive { get; set; }
    
    public MentorPerformance MentorPerformance { get; set; }
    
    public MentorSettings MentorSettings { get; set; }
    
    public long? MentorActiveStudents { get; set; }
}