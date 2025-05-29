namespace Sefer.Backend.Api.Controllers.Mentor;

[Authorize(Roles = "Mentor")]
public class ChatManagementController(IServiceProvider serviceProvider)  : UserController(serviceProvider)
{
    private readonly IWebSocketProvider _webSocket = serviceProvider.GetService<IWebSocketProvider>();
    
    [HttpPost("/mentor/channels")]
    public async Task<ActionResult> CreateChannel([FromBody] ChannelPostModel model)
    {
        // Check for correct model
        if (model == null || (model.Name ?? string.Empty).Length < 3) return BadRequest();
        
        // Ensure the current user is a mentor
        var mentor = await GetCurrentUser();
        if (mentor is not { Role: UserRoles.Mentor }) return Forbid();
        
        // Check if the name of the channel already exists
        var channels = await Send(new GetChannelsRequest(mentor.Id));
        var exists = channels.Any(c => c.Name == model.Name && c.Type == ChannelTypes.Private);
        if(exists) return BadRequest();
        
        // Create the channel
        var request = new CreateGroupChannelRequest(mentor.Id, model.Name);
        var channel = await Send(request);
        if(channel == null) return Forbid();
        
        // Add all the posted students
        channel = await Send(new AddChannelReceiverRequest(channel.Id, model.Students));
        if (channel == null) return BadRequest();
        
        // Technically, when channel is created, also it's receivers change
        // (and an easy way to reuse the complex code in the front-end)
        var current = await Send(new GetUsersInChannelRequest(channel.Id));
        await _webSocket.ChannelReceiversChanged(channel.Id, [], current);
        
        return Json(new ChannelView(channel));
    }

    [HttpPut("/mentor/channels/{id:int}")]
    public async Task<ActionResult> UpdateChannel([FromBody] ChannelPostModel model, int id)
    {
        // Basic check for the model        
        if (model == null || (model.Name ?? string.Empty).Length < 3) return BadRequest();
        
        // Check if the mentor is a receiver of this channel
        var mentor = await GetCurrentUser();
        var isReceiver = await Mediator.Send(new IsUserInChannelRequest(id, mentor.Id ));
        if(!isReceiver) return BadRequest();
        
        // Update the channel
        var previous = await Send(new GetUsersInChannelRequest(id));
        var channel = await Mediator.Send(new UpdateChannelRequest(id, mentor.Id, model.Name, model.Students));
        if (channel == null) return BadRequest();
        
        // Ensure to update any current online user in the channel
        var current = await Send(new GetUsersInChannelRequest(id));
        await _webSocket.ChannelReceiversChanged(channel.Id, previous, current);
        
        // Return the view to the mentor
        return Json(new ChannelView(channel));
    }
    
    [HttpPost("/mentor/channels/add-students")]
    public async Task<ActionResult> AddStudentToChannel(ChannelStudentPostModel model)
    {
        // Check if channel and student provided are valid given the current mentor
        var isValid = await ValidateChannelStudent(model);
        if (!isValid) return BadRequest();
        var previous = await Send(new GetUsersInChannelRequest(model.ChannelId));
        
        // Ensure the student is added to the channel and return the result
        var channel = await Send(new AddChannelReceiverRequest(model.ChannelId, model.Students));
        if (channel == null) return BadRequest();
        
        // Ensure to update any current online user in the channel
        var current = await Send(new GetUsersInChannelRequest(model.ChannelId));
        await _webSocket.ChannelReceiversChanged(channel.Id, previous, current);
        
        // Return the view to the mentor
        return Json(new ChannelView(channel));
    }
    
    [HttpPost("/mentor/channels/remove-students")]
    public async Task<ActionResult> RemoveStudentFromChannel(ChannelStudentPostModel model)
    {
        // Check if channel and student provided are valid given the current mentor
        var isValid = await ValidateChannelStudent(model);
        if (!isValid) return BadRequest();
        var previous = await Send(new GetUsersInChannelRequest(model.ChannelId));

        // Ensure the student is added to the channel and return the result
        var channel = await Send(new RemoveChannelReceiverRequest(model.ChannelId, model.Students));
        if (channel == null) return BadRequest();
        
        // Ensure to update any current online user in the channel
        var current = await Send(new GetUsersInChannelRequest(model.ChannelId));
        await _webSocket.ChannelReceiversChanged(channel.Id, previous, current);
        
        // Return the view to the mentor
        return Json(new ChannelView(channel));
    }

    [HttpDelete("/mentor/channels/{channelId:int}")]
    public async Task<ActionResult> DeleteChannel(int channelId)
    {
        // Ensure the current user is a mentor
        var mentor = await GetCurrentUser();
        if (mentor is not { Role: UserRoles.Mentor }) return Forbid();

        // Delete the group and return the result
        var previous = await Send(new GetUsersInChannelRequest(channelId));
        var request = new DeletePrivateChannelRequest(mentor.Id, channelId);
        var deleted = await Mediator.Send(request);
        if(!deleted) return BadRequest();
        
        // Technically, when channel is deleted, also it's receivers change
        // (and an easy way to reuse the complex code in the front-end)
        await _webSocket.ChannelReceiversChanged(channelId, previous, []);

        return Ok();
    }
    
    private async Task<bool> ValidateChannelStudent(ChannelStudentPostModel model)
    {
        // Check if provided student is a student of the current user
        var mentor = await GetCurrentUser();
        foreach (var student in model.Students)
        {
            var isStudent = await Send(new IsStudentOfMentorRequest(mentor.Id, student));
            if (!isStudent) return false;    
        }
        
        // Check given channel exists and if the mentor is that channel
        var users = await Send(new GetUsersInChannelRequest(model.ChannelId));
        return users.Any(u => u.Id == mentor.Id);
    }
}