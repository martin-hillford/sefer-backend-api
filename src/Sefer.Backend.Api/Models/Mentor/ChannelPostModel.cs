// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Mentor;

public class ChannelPostModel
{
    /// <summary>
    /// The name of the channel
    /// </summary>
    [MinLength(3)]
    public string Name { get; set; }
    
    /// <summary>
    /// Contains a list of all the ids of the students to be in channel
    /// </summary>
    public List<int> Students { get; set; }
}