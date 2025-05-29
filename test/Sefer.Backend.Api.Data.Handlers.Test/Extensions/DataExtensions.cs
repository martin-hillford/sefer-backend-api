namespace Sefer.Backend.Api.Data.Handlers.Test.Extensions;

public static class DataExtensions
{
    public static async Task<User> GetStudent(this DataContext context) =>
        await context.Users.SingleAsync(u => u.Role == UserRoles.Student);

    public static async Task<User> GetMentor(this DataContext context) =>
        await context.Users.SingleAsync(u => u.Role == UserRoles.Mentor);
}