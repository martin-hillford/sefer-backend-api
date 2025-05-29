namespace Sefer.Backend.Api.Chat.Test;

public class MockedServiceProvider
{
    private readonly Mock<IServiceProvider> _mocked = new();

    private readonly Mock<IMediator> _mediator = new();
    
    private readonly Mock<INotificationService> _notificationService = new();
    
    public readonly Mock<IChatHubContext> ChatContext = new();
    
    public readonly Mock<IHubCallerClients> Clients = new();
    
    private readonly Mock<ISingleClientProxy> _caller = new();
    
    public readonly Mock<IGroupManager> Groups = new();
    
    public MockedServiceProvider AddRequestResult<TRequest, TResponse>(TResponse response)
        where TRequest : IRequest<TResponse>
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        return this;
    }

    public MockedServiceProvider AddRequestResults<TRequest, TResponse>(List<TResponse> responses)
        where TRequest : IRequest<TResponse>
    {
        var setup = _mediator.SetupSequence(m => m.Send(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()));
        var result = setup.ReturnsAsync(responses.First());

        for (var index = 1; index < responses.Count; index++)
        {
            result = result.ReturnsAsync(responses[index]);
        }

        return this;
    }

    public IServiceProvider Object
    {
        get
        {
            Clients.Setup(c => c.Caller).Returns(_caller.Object);
            ChatContext.Setup(c => c.GetClients()).Returns(Clients.Object);
            ChatContext.Setup(c => c.GetGroups()).Returns(Groups.Object);
            
            _mocked.Setup(s => s.GetService(typeof(IMediator))).Returns(_mediator.Object);
            _mocked.Setup(s => s.GetService(typeof(INotificationService))).Returns(_notificationService.Object);
            _mocked.Setup(s => s.GetService(typeof(IChatHubContext))).Returns(ChatContext.Object);
            return _mocked.Object;
        }
    }

    public static MockedServiceProvider Create() => new();
}