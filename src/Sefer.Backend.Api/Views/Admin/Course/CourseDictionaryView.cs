// ReSharper disable MemberCanBePrivate.Global MemberCanBeProtected.Global UnusedMember.Global
namespace Sefer.Backend.Api.Views.Admin.Course;

public class CourseDictionaryView(Data.Models.Courses.Course course, Revision revision, List<CourseRevisionDictionaryWord> words)
    : Api.Models.Admin.Course.CourseDictionary(revision.Id, words)
{
    public string CourseName => course.Name;
    
    public int CourseId => course.Id;
    
    public int CourseRevisionVersion => revision.Version;
    
    public readonly Data.JsonViews.CourseView Course = new(course);
}