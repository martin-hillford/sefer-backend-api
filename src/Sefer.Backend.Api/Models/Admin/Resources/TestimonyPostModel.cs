// This is a post model. And although it is never instantiated in code, it is in runtime 
// ReSharper disable UnusedAutoPropertyAccessor.Global ClassNeverInstantiated.Global
using DataModel = Sefer.Backend.Api.Data.Models.Resources.Testimony;

namespace Sefer.Backend.Api.Models.Admin.Resources;

/// <summary>
/// This class is a testimony model that can posted to insert or update
/// </summary>
public class TestimonyPostModel
{
    /// <summary>
    /// The (optional) id of the course of which this testimony is about
    /// </summary>
    public int? CourseId { get; set; }

    /// <summary>
    /// The optional id of the student that the testimony.
    /// </summary>
    public int? StudentId { get; set; }

    /// <summary>
    /// The survey result this testimony is based on
    /// </summary>
    public int? SurveyResultId { get; set; }

    /// <summary>
    /// The testimony itself
    /// </summary>
    [Required, MinLength(2)]
    public string Content { get; set; }

    /// <summary>
    /// The (optional) name of the person who made this testimony
    /// </summary>
    [MaxLength(255)]
    public string Name { get; set; }

    /// <summary>
    /// When true the website will display this as anonymous
    /// </summary>
    public bool IsAnonymous { get; set; }

    public DataModel ToDataModel()
    {
        return new DataModel
        {
            Content = Content,
            StudentId = StudentId,
            Name = Name,
            SurveyResultId = SurveyResultId,
            IsAnonymous = IsAnonymous,
            CourseId = CourseId,
        };
    }
}