using Sefer.Backend.Api.Chat.Views;
using Sefer.Backend.Api.Data.Models.Users.Chat;
using Sefer.Backend.Api.Models.Users;
using Sefer.Backend.Api.Views.Chat;
using ChannelView = Sefer.Backend.Api.Views.Shared.Users.Chat.ChannelView;

namespace Sefer.Backend.Api.Controllers.Users;

[Authorize(Roles = "Student,User,Admin,Mentor,Supervisor")]
public class ChatMessageController(IServiceProvider provider) : UserController(provider)
{
    private const int NumberOfMessage = 50;

    private readonly INotificationService _notificationService = provider.GetService<INotificationService>();

    [HttpGet("/user/channels/{channelId:int}/messages")]
    [ProducesResponseType(typeof(List<MessageView>), 200)]
    public async Task<ActionResult> GetMessages(int channelId, int skip = 0, int take = NumberOfMessage)
    {
        // check if the channel can be found (404)
        var channel = await Send(new GetChannelByIdRequest(channelId));
        if (channel == null) return NotFound();

        // check if the user can be found (401)
        var user = await GetCurrentUser();
        if (user == null) return Unauthorized();

        // check if the user has access to the channel (401)
        var isInChannel = await Send(new IsUserInChannelRequest(channel.Id, user.Id));
        if (!isInChannel) return Unauthorized();

        // Load the first 500 messages
        var messages = await Send(new GetChatMessagesRequest(channel.Id, skip, take));

        // Create the view on each of the messages
        var view = messages.Select(message => new ChatMessageView(message, channel.Type, user.Id));

        // Return the message
        return Json(view);
    }

    [HttpPost("/user/channels/{channelId:int}/messages")]
    [ProducesResponseType(typeof(MessageView), 202)]
    public async Task<ActionResult> PostMessage([FromBody] MessagePostModel message, int channelId)
    {
        // Check if the model is valid
        if (message == null || ModelState.IsValid == false) return BadRequest();
        if (message.ChannelId != channelId) return BadRequest();

        // check if the channel can be found (404)
        var channel = await Send(new GetChannelByIdRequest(message.ChannelId));
        if (channel == null) return NotFound();

        // check if the user can be found (401)
        var user = await GetCurrentUser();
        if (user == null) return Unauthorized();

        // check if the user has access to the channel (401)
        var isInChannel = await Send(new IsUserInChannelRequest(channel.Id, user.Id));
        if (!isInChannel) return Unauthorized();

        // Post the message and return the result
        var postRequest = new PostTextChatMessageRequest
        {
            Text = message.Content,
            ChannelId = channel.Id,
            SenderId = user.Id,
            QuotedMessageId = message.QuotedMessageId,
            QuotedMessageText = message.QuotedMessage
        };
        var posted = await Send(postRequest);
        if (posted == null) return StatusCode(500);
        var view = new MessageView(posted, channel.Type, user.Id);

        // Also post the message via the traditional way
        await Send(new LoadMessageReferencesRequest(posted));

        var task = posted.ChannelMessages.Select(v => _notificationService.SendChatMessageSendNotificationAsync(v, true));
        await Task.WhenAll(task);

        // The user has posted his message
        return Json(view, 202);
    }

    [HttpGet("/user/messages/count")]
    public async Task<ActionResult> GetTotalMessagesCount()
    {
        var user = await GetCurrentUser();
        if (user == null) return Unauthorized();

        var count = await Send(new GetTotalUnreadMessagesRequest(user.Id));
        var view = new CountView { Count = count };
        return Json(view);
    }

    [HttpPut("/user/messages/{messageId:int}/mark-read")]
    public async Task<ActionResult> MarkMessageAsRead(int messageId)
    {
        // check if the user can be found (401)
        var user = await GetCurrentUser();
        if (user == null) return Unauthorized();

        var marked = await Send(new MarkMessageAsReadRequest(messageId, user.Id));
        if (!marked) return StatusCode(500);

        await _notificationService.SendChatMessageIsReadNotificationAsync(messageId, user);
        return Ok();
    }

    /// <summary>
    /// Searches for chat messages
    /// </summary>
    /// <response code="401">No user authenticated</response>
    /// <response code="400">The search terms is missing or less than 3 characters</response>
    [HttpPost("/user/channels/search-messages")]
    [ProducesResponseType(typeof(List<ChatSearchResult>), 200)]
    public async Task<ActionResult> Search([FromBody] MessageSearchPostModel body)
    {
        // check if the user can be found (401)
        var user = await GetCurrentUser();
        if (user == null) return Unauthorized();

        // check for valid input
        if (!ModelState.IsValid) return BadRequest();

        // Search for messages
        var request = new SearchChatMessagesRequest(user.Id, body.Term);
        var result = await Send(request);

        // Return result to user
        return Json(result);
    }

    [HttpGet("/user/messages/{messageId}/channel")]
    public async Task<ActionResult> GetChannelForMessage(int messageId)
    {
        // check if the user can be found (401)
        var user = await GetCurrentUser();
        if (user == null) return Unauthorized();

        // Search for messages
        var request = new GetMessageByIdRequest(user.Id, messageId);
        var result = await Send(request);
        if (result == null) return NotFound();

        var view = new ChannelView(result.Channel);
        return Json(view);
    }
}