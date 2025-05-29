// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.JsonViews;

public class CourseStat
{
    public Course Course { get; init; }
    
    public int Students { get; init; }
    
    public int Lessons { get; init; }
}