// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Mentor;

/// <summary>
/// The remarks of the mentor on the student
/// </summary>
public class MentorRemarksPostModel
{
    public int StudentId { get; set; }

    public string Remarks { get; set; }
}