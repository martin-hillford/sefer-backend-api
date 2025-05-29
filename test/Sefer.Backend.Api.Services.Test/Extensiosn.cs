namespace Sefer.Backend.Api.Services.Test;

public static class Extensions
{
    public static Mock<IMediator> SetupRequest<TRequest, TResponse>(this Mock<IMediator> mediator, TResponse response)
        where TRequest : IRequest<TResponse>
    {
        mediator
            .Setup(m => m.Send(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        return mediator;
    }
}