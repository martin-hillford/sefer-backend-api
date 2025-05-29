// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Mentor;

public class ChannelStudentPostModel
{
    public List<int> Students { get; set; }
    
    public int ChannelId { get; set; }
}