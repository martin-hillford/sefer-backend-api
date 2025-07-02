// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global, CollectionNeverQueried.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Models.Public.Download;

public class Course(Data.Models.Courses.Course course, CourseRevision revision)
{
    #region Properties
    
    public int Id => course.Id;
    
    public DateTime CreationDate  => course.CreationDate;
    
    public DateTime? ModificationDate => course.ModificationDate;
    
    public string Name => course.Name;
    
    public string Permalink => course.Permalink;
    
    public string Description => course.Description;
    
    public string Author => course.Author;
    
    public string Citation => course.Citation;
    
    public string WebshopLink => course.WebshopLink;
    
    public string VideoIntroductionLink => course.IntroductionLink;
    
    public string Copyright => course.Copyright;
    
    public string CopyrightLogo => course.CopyrightLogo;
    
    public int? MaxLessonSubmissionsPerDay => course.MaxLessonSubmissionsPerDay;
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Levels Level => course.Level;
    
    public bool IsVideoCourse => course.IsVideoCourse;

    public bool ShowOnHomePage => course.ShowOnHomepage;
    
    public bool Private => course.Private;
    
    public bool AllowSelfStudy => revision.AllowSelfStudy;
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Stages Stage => revision.Stage;
    
    public int Version => revision.Version;
    
    public Images Images => new(course);
    
    #endregion
    
    #region Collections

    public List<Lesson> Lessons = [];

    #endregion
}