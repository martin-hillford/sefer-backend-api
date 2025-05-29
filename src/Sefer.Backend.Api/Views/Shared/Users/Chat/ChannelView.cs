using Sefer.Backend.Api.Data.JsonViews;
using Sefer.Backend.Api.Data.Models.Users.Chat;

namespace Sefer.Backend.Api.Views.Shared.Users.Chat;

/// <summary>
/// A view on the channel
/// </summary>
public class ChannelView : AbstractView<Channel>
{
    /// <summary>
    /// The DateTime this channel was created
    /// </summary>
    public DateTime CreationDate => Model.CreationDate;

    /// <summary>
    /// The type of the channel
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ChannelTypes Type => Model.Type;
    
    /// <summary>
    /// The name of the channel. Only applicable for none-personal channels
    /// </summary>
    public string Name => Model.Name;

    /// <summary>
    /// Creates new view based on a model
    /// </summary>
    public ChannelView(Channel model) : base(model) { }
}