// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Users;

public class DonationPromisePostModel
{
    public int Amount { get; set; }

    public string Description { get; set; }
    
    public string Site { get; set; }
}