// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Test;

public class MockedServiceProvider
{
    private readonly Mock<IServiceProvider> _mocked = new();

    public readonly Mock<IMediator> Mediator = new();

    public MockedServiceProvider AddRequestResult<TRequest, TResponse>(TResponse response)
        where TRequest : IRequest<TResponse>
    {
        Mediator
            .Setup(m => m.Send(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        return this;
    }
    
    public MockedServiceProvider AddRequestResult<TRequest, TResponse>(dynamic parameters, string property)
        where TRequest : IRequest<TResponse>
        where TResponse : class 
    {
        var response = Extensions.GetValueOrNull(parameters, property);
        if (response is null) return this;
        
        Mediator
            .Setup(m => m.Send(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response  as TResponse);

        return this;
    }

    public MockedServiceProvider AddRequestResults<TRequest, TResponse>(List<TResponse> responses)
        where TRequest : IRequest<TResponse>
    {
        var setup = Mediator.SetupSequence(m => m.Send(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()));
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
            _mocked.Setup(s => s.GetService(typeof(IMediator))).Returns(Mediator.Object);
            return _mocked.Object;
        }
    }

    public ISetup<IServiceProvider, TResult> Setup<TResult>(Expression<Func<IServiceProvider, TResult>> expression)
    {
        return _mocked.Setup(expression);
    }

    public MockedServiceProvider AddService<TInterface, TService>(TService service) where TService : TInterface
    {
        _mocked.Setup(s => s.GetService(typeof(TInterface))).Returns(service);
        return this;
    }

    public MockedServiceProvider AddService<TInterface>(Mock<TInterface> service) where TInterface : class
    {
        _mocked.Setup(s => s.GetService(typeof(TInterface))).Returns(service.Object);
        return this;
    }

    public MockedServiceProvider SetupUserId(int? userId)
    {
        var service = new Mock<IUserAuthenticationService>();
        service.Setup(s => s.UserId).Returns(userId);
        return AddService(service);
    }

    public MockedServiceProvider SetupUser(User user)
    {
        return SetupUserId(user.Id).AddRequestResult<GetUserByIdRequest, User>(user);
    }

    public static MockedServiceProvider Create() => new();
}

public static class MockedMediatorExtensions
{
    public static void VerifySend<TRequest, TResponse>(this MockedServiceProvider provider, Times times)
        where TRequest : IRequest<TResponse>
    {
        VerifySend<TRequest, TResponse>(provider.Mediator, times);
    }
    
    public static void VerifySend<TRequest, TResponse>(this Mock<IMediator> mediator, Times times) where TRequest : IRequest<TResponse>
    {
        mediator.Verify(m => m.Send(It.IsAny<TRequest>(), It.IsAny<CancellationToken>() ), times);
    }
}