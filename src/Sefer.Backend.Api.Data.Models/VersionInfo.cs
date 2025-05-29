// This is an entity framework model so some properties may not be set in code
// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Sefer.Backend.Api.Data.Models;

[Table("__migrations")]
public class VersionInfo
{
    public int Number { get; set; }

    public DateTime Date { get; set; }

    [MaxLength(255)]
    public string Name { get; set; }
}