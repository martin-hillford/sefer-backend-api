// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Models.Public.Download;

[JsonDerivedType(typeof(TextElement))]
[JsonDerivedType(typeof(BoolQuestion))]
[JsonDerivedType(typeof(MediaElement))]
[JsonDerivedType(typeof(OpenQuestion))]
[JsonDerivedType(typeof(MultipleChoiceQuestion))]
public abstract class ContentBlock(Data.Models.Courses.Lessons.ContentBlock contentBlock)
{
    public int Id => contentBlock.Id;
    
    public DateTime CreationDate  => contentBlock.CreationDate;
    
    public DateTime? ModificationDate => contentBlock.ModificationDate;
    
    public int SequenceId => contentBlock.SequenceId;

    public string Number => contentBlock.Number;
    
    public bool ForcePageBreak => contentBlock.ForcePageBreak;
    
    public string Heading => contentBlock.Heading;
    
    public int? PredecessorId => contentBlock.PredecessorId;
}