// ReSharper disable MemberCanBePrivate.Global MemberCanBeProtected.Global
namespace Sefer.Backend.Api.Models.Admin.Course;

public class CourseDictionary
{
    public int CourseRevisionId { get; set; }
    
    public List<CourseDictionaryWord> Words { get; set; }
    
    public CourseDictionary(int courseRevisionId, List<CourseRevisionDictionaryWord> words)
    {
        CourseRevisionId = courseRevisionId;
        Words = words.Select(x => new CourseDictionaryWord(x)).ToList();
    }
    
    /// <summary>
    /// Constructor for json deserialization
    /// </summary>
    [JsonConstructor]
    public CourseDictionary() { }

    internal List<CourseRevisionDictionaryWord> GetWordList()
    {
        return Words.Select(GetModel).ToList();
    }

    private CourseRevisionDictionaryWord GetModel(CourseDictionaryWord word)
    {
        return new CourseRevisionDictionaryWord()
        {
            CourseRevisionId = CourseRevisionId,
            Explanation = word.Explanation,
            Language = word.Language,
            PictureUrl = word.PictureUrl,
            Word = word.Word
        };
    }
}

public class CourseDictionaryWord
{
    [Required, MinLength(2), MaxLength(255)]
    public string Word { get; set; }
    
    [Required, MinLength(5), MaxLength(int.MaxValue)]
    public string Explanation { get; set; }
    
    [Required, MinLength(2), MaxLength(3)]
    public string Language { get; set; }
    
    [MaxLength(int.MaxValue)]
    public string PictureUrl { get; set; }
        
    /// <summary>
    /// Constructor for json deserialization
    /// </summary>
    [JsonConstructor]
    public CourseDictionaryWord() { }

    public CourseDictionaryWord(CourseRevisionDictionaryWord word)
    {
        Word = word.Word;
        Explanation = word.Explanation;
        Language = word.Language;
        PictureUrl = word.PictureUrl;
    }
}
