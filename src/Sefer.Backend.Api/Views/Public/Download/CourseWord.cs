namespace Sefer.Backend.Api.Views.Public.Download;

public class CourseWord(CourseRevisionDictionaryWord data)
{
    public string Word => data.Word;
    
    public string Explanation => data.Explanation;
    
    public string Language => data.Language;
    
    public string PictureUrl { get; set; } = data.PictureUrl;
}