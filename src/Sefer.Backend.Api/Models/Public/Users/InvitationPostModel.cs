// This is a post model. And although it is never instantiated in code, it is in runtime 
// ReSharper disable UnusedAutoPropertyAccessor.Global ClassNeverInstantiated.Global
namespace Sefer.Backend.Api.Models.Public.Users;

public class InvitationPostModel
{
    public string Date { get; set; }

    public string Hash { get; set; }

    public string MentorId { get; set; }

    public string Random { get; set; }

    public (string, string, string, string) GetParams()
    {
        return (MentorId, Random, Date, Hash);
    }

    public int GetMentorId()
    {
        var isInt = int.TryParse(MentorId, out var result);
        return isInt ? result : 0;
    }
}