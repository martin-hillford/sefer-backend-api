namespace Sefer.Backend.Api.Data.Handlers.Test.ChatMessages;

[TestClass]
public class PostEnrollmentChatMessageHandlerTest : PostChatMessageHandlerTest
{
    [TestMethod]
    public async Task Handle_EnrollmentNull()
    {
        var message = await Handle(null, null);
        message.Should().BeNull();

    }
    
    [TestMethod]
    public async Task Handle_SelfStudyEnrollment()
    {
        var enrollment = new Enrollment { MentorId = null }; 
        var message = await Handle(enrollment, null);
        message.Should().BeNull();
    }

    [TestMethod]
    public async Task Handle_NoChannel()
    {
        var enrollment = new Enrollment { MentorId = 1 }; 
        var message = await Handle(enrollment, null);
        message.Should().BeNull(); 
    }

    [TestMethod]
    public async Task Handle()
    {
        var course = new Course { Name = "name" };
        var revision = new CourseRevision { Course = course };
        var enrollment = new Enrollment { MentorId = 1, CourseRevision = revision };
        var channel = new Channel();
        var message = await Handle(enrollment, channel);
        message.Should().NotBeNull(); 
    }
    
    private async Task<Message> Handle(Enrollment? enrollment, Channel? channel)
    {
        var provider = GetServiceProvider();
        if(enrollment != null) provider.AddRequestResult<GetEnrollmentByIdExtensivelyRequest, Enrollment>(enrollment);
        if(channel != null) provider.AddRequestResult<GetPersonalChannelRequest, Channel>(channel);
        provider.AddRequestResult<PostObjectChatMessageRequest, Message>(new Message());
        
        var request = new PostEnrollmentChatMessageRequest(13);
        var handler = new PostEnrollmentChatMessageHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}