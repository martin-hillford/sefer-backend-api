// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
using Sefer.Backend.Api.Chat.Views;
using Sefer.Backend.Api.Data.Models.Users.Chat;

namespace Sefer.Backend.Api.Views.Chat;

public class ChatMessageView : MessageView
{
    public readonly DateTime? ReadDate;
    
    public bool IsRead => ReadDate.HasValue;
    
    public readonly bool? ReadByAll;
    
    public ChatMessageView(Message message, ChannelTypes channelType, int userId) : base(message, channelType, userId)
    {
        ReadDate = 
            message.SenderId == userId 
                ? message.SenderDate 
                : message.ChannelMessages.SingleOrDefault(c => c.ReceiverId == userId)?.ReadDate;
        ReadByAll = message.ChannelMessages?.All(m => m.ReadDate.HasValue);
    }
}