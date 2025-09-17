// As this is a view, the get method of the properties may only be used by the JSON serialization
// ReSharper disable MemberCanBePrivate.Global, UnusedMember.Global, MemberCanBeProtected.Global

using Sefer.Backend.Api.Models.Public;

namespace Sefer.Backend.Api.Views.Public.Download;

[JsonDerivedType(typeof(TextElement))]
[JsonDerivedType(typeof(BoolQuestion))]
[JsonDerivedType(typeof(MediaElement))]
[JsonDerivedType(typeof(OpenQuestion))]
[JsonDerivedType(typeof(MultipleChoiceQuestion))]
public abstract class ContentBlock(Data.Models.Courses.Lessons.ContentBlock contentBlock)
{
    public int Id => contentBlock.Id;

    public long CreationDate => contentBlock.CreationDate.ToUnixTime();
    
    public long? ModificationDate => contentBlock.ModificationDate?.ToUnixTime();
    
    public int SequenceId => contentBlock.SequenceId;

    public string Number => contentBlock.Number;
    
    public bool ForcePageBreak => contentBlock.ForcePageBreak;
    
    public string Heading => contentBlock.Heading;
    
    public int? PredecessorId => contentBlock.PredecessorId;
    
    public virtual async Task IncludeMedia(DownloadRequest request, Course course)
    {
        var images = ContentSupport.FindImageUrls(Content);
        foreach (var imageUrl in images)
        {
            var resource = await ContentSupport.CreateResource(request, imageUrl);
            if (resource == null) continue;
            course.Resources.Add(resource);
            Content = Content.Replace(imageUrl, resource.GetResourceUrl());
        }
    }

    public abstract string Content { get; set; }
}