// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Models.Public.Download;

public class Lesson(Data.Models.Courses.Lessons.Lesson lesson)
{
    public int Id => lesson.Id;
    
    public DateTime CreationDate  => lesson.CreationDate;
    
    public DateTime? ModificationDate => lesson.ModificationDate;
    
    public int SequenceId => lesson.SequenceId;
    
    public string Number => lesson.Number;
    
    public string Name => lesson.Name;
    
    public string Description => lesson.Description;
    
    public string ReadBeforeStart => lesson.ReadBeforeStart;
    
    public int? PredecessorId => lesson.PredecessorId;
    
    public Guid? AudioReferenceId => lesson.AudioReferenceId;

    public ReadOnlyCollection<ContentBlock> Content => _contentBlocks.OrderBy(c => c.SequenceId).ToList().AsReadOnly();

    private readonly List<ContentBlock> _contentBlocks = [];
    
    public void AddBlocks<T>(IEnumerable<T> blocks) where T : ContentBlock
    {
        _contentBlocks.AddRange(blocks);
    }
}