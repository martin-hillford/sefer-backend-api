// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Services.Newsletter.CleverReach;

public class TokenResponse
{
    // Please note, the prop name is not according to C# naming conventions because of the clever reach api
    // ReSharper disable once InconsistentNaming
    public string access_token { get; set; }
}