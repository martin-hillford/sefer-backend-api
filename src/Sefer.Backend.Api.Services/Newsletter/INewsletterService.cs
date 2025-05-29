namespace Sefer.Backend.Api.Services.Newsletter;

public interface INewsletterService
{
    public Task<bool> Subscribe(string name, string email);

    public Task<bool> IsSubscribed(string email);

    public Task<bool> UnSubscribe(string email);
}