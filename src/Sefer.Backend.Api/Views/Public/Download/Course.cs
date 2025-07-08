// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global, CollectionNeverQueried.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global, UnusedMember.Global

using Sefer.Backend.Api.Models.Public;

namespace Sefer.Backend.Api.Views.Public.Download;

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
    
    public string CopyrightLogo { get; set; } = course.CopyrightLogo;
    
    public int? MaxLessonSubmissionsPerDay => course.MaxLessonSubmissionsPerDay;
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Levels Level => course.Level;
    
    public bool IsVideoCourse => course.IsVideoCourse;

    public bool ShowOnHomePage => course.ShowOnHomepage;
    
    public bool Private => course.Private;
    
    public bool AllowSelfStudy => revision.AllowSelfStudy;
    
    public string GeneralInformation => revision.GeneralInformation;
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Stages Stage => revision.Stage;
    
    public int Version => revision.Version;

    public Images Images { get; set; } = new (course);

    #endregion
    
    #region Collections

    public List<Lesson> Lessons = [];

    public List<Resource> Resources = [];

    #endregion

    #region Methods

    public async Task IncludeMedia(DownloadRequest request)
    {
        await Images.IncludeMedia(request, this);
        var tasks = Lessons.SelectMany(l => l.Content.Select(c => c.IncludeMedia(request, this)));
        await Task.WhenAll(tasks.ToArray());
        
        var copyrightLogo = await ContentSupport.CreateResource(request, CopyrightLogo);
        if (copyrightLogo != null) CopyrightLogo = copyrightLogo.GetResourceUrl();
        if (copyrightLogo != null) Resources.Add(copyrightLogo);
    }

    #endregion
}