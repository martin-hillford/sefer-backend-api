using System.Linq;

namespace Sefer.Backend.Api.Data.Requests.Courses;

public class SaveCourseDictionaryRequest : IRequest<bool>
{
    public readonly ReadOnlyDictionary<string, CourseRevisionDictionaryWord> Words;

    public readonly int CourseRevisionId; 

    public SaveCourseDictionaryRequest(int courseRevisionId, List<CourseRevisionDictionaryWord> words)
    {
        Words = words.ToDictionary(w => w.Word.ToLower().Trim(), w => w).AsReadOnly();
        CourseRevisionId = courseRevisionId;
    }
}