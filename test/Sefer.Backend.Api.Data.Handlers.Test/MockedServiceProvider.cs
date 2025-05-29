using Microsoft.Extensions.Caching.Memory;

namespace Sefer.Backend.Api.Data.Handlers.Test;

public class MockedServiceProvider
{
    private readonly Mock<IServiceProvider> _mocked = new();

    public readonly Mock<IMediator> Mediator = new();

    public MockedServiceProvider AddDataContextProvider(IDataContextProvider provider)
    {
        _mocked.Setup(p => p.GetService(typeof(IDataContextProvider))).Returns(provider);
        return this;
    }

    public MockedServiceProvider AddValidationService<T>(Mock<ICustomValidationService<T>> service)
    {
        _mocked
            .Setup(p => p.GetService(typeof(ICustomValidationService<T>)))
            .Returns(service.Object);
        return this;
    }

    public MockedServiceProvider AddRequestResult<TRequest, TResponse>(TResponse response)
        where TRequest : IRequest<TResponse>
    {
        Mediator
            .Setup(m => m.Send(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        return this;
    }

    public MockedServiceProvider AddRequestException<TRequest, TResponse>()
        where TRequest : IRequest<TResponse>
    {
        Mediator
            .Setup(m => m.Send(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        return this;
    }

    public MockedServiceProvider AddRequestResults<TRequest, TResponse>(params TResponse?[] responses)
        where TRequest : IRequest<TResponse>
        => AddRequestResults<TRequest, TResponse?>(responses.ToList());

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

    public MockedServiceProvider AddService<TInterface, TService>(TService service) where TService : TInterface
    {
        _mocked.Setup(s => s.GetService(typeof(TInterface))).Returns(service);
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

    public MockedServiceProvider AddCaching()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        _mocked.Setup(s => s.GetService(typeof(IMemoryCache))).Returns(cache);
        return this;
    }

    public void SetupMentorAssigning(User mentor)
    {
        var assigner = new Mock<IMentorAssigningAlgorithm>();
        assigner.Setup(c => c.GetMentor()).Returns(mentor);
        var factory = new Mock<IMentorAssigningFactory>();
        factory.Setup(m => m.PrepareAlgorithm(It.IsAny<MentorAssigningInput>())).Returns(assigner.Object);
        AddService<IMentorAssigningFactory, IMentorAssigningFactory>(factory.Object);
    }
}