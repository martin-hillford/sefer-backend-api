// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Admin.Curriculum;

/// <summary>
/// A structure to post the sequence of a block
/// </summary>
public class CurriculumBlockSequencePostModel
{
    /// <summary>
    /// The id of curriculum these blocks are in
    /// </summary>
    public int CurriculumId { get; set; }

    /// <summary>
    /// The year the blocks are in
    /// </summary>
    public short? Year { get; set; }

    /// <summary>
    /// The blocks (ids) to be saved, sequence is implicit
    /// </summary>
    public List<int> Blocks { get; set; }
}
