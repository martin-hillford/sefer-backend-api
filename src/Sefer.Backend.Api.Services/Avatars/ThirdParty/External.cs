using Sefer.Backend.Api.Services.HttpConnection;

namespace Sefer.Backend.Api.Services.Avatars.ThirdParty;

public class External(IHttpClient client)
{
    private readonly Gravatar _gravatar = new(client);

    private readonly Libravatar _libravater = new(client);
    
    private static string GravatarHash(string email)
    {
        var bytes = Encoding.ASCII.GetBytes(email.Trim().ToLower());
        var hash = MD5.HashData(bytes);
        return BitConverter.ToString(hash).ToLower().Replace("-", string.Empty);
    }

    public async Task<Response> Gravatar(string email, int size)
    {
        var hash = GravatarHash(email);
        return await _gravatar.Retrieve(hash, size);
    }
    
    public async Task<Response> Libravater(string email, int size)
    {
        var hash = GravatarHash(email);
        return await _gravatar.Retrieve(hash, size);
    }
    
    public async Task<Response> Retrieve(string email, int size)
    {
        var hash = GravatarHash(email);
        return await _gravatar.Retrieve(hash, size) ??
               await _libravater.Retrieve(hash, size);
    }
}