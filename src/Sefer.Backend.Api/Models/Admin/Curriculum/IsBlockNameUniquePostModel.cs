// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Admin.Curriculum;

/// <summary>
/// This model is used to post request on the uniqueness of the name of a block
/// </summary>
public class IsBlockNameUniquePostModel
{
    /// <summary>
    /// The id of block (omitted for new blocks)
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// The year in which this block is located (when smaller than 1 no years are assumed)
    /// </summary>
    public short? Year { get; set; }

    /// <summary>
    /// The curriculum this block belongs to (a comparable revision system is used as with the courses)
    /// </summary>
    public int CurriculumId { get; set; }

    /// <summary>
    /// The name for the block
    /// </summary>
    public string Name { get; set; }
}
