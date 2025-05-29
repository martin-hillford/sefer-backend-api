namespace Sefer.Backend.Api.Data.Models.Abstractions;

/// <summary>
/// Generic element definition (include content block, course and survey questions)
/// </summary>
/// <inheritdoc cref="IModifyDateLogEntity"/>
public interface IElement : IModifyDateLogEntity
{
    /// <summary>
    /// Gets / Sets the sequence number for this ContentBlock. Does not have to be unique. 
    /// But it will be used for sorting the lessons within a Lesson
    /// </summary>
    int SequenceId { get; set; }

    /// <summary>
    /// A number for this element. Can be used to separate a lesson in parts (optional).
    /// </summary>
    string Number { get; set; }

    /// <summary>
    /// When true, a page break will be forced for the user
    /// </summary>
    bool ForcePageBreak { get; set; }

    /// <summary>
    /// A heading for this element (optional).
    /// </summary>
    string Heading { get; set; }

    /// <summary>
    /// Get a format text for a header (combination of number and heading)
    /// </summary>
    string HeaderText { get; }
}