namespace Sefer.Backend.Api.Services.Avatars;

public static class Unknown
{
    public static Response Create(string initials, string fill, string color)
    {
        var image = $"""<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" height="64" width="64"><circle cx="50%" cy="50%" r="32" fill="{fill}" /><text letter-spacing="2" x="50%" y="50%" style="color: {color}; line-height: 1;font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Fira Sans', 'Droid Sans', 'Helvetica Neue', sans-serif;" alignment-baseline="middle" text-anchor="middle" font-size="28" font-weight="400" dy=".1em" dominant-baseline="middle" fill="{color}">{initials}</text></svg>""";
        return Response.FromString(image, "image/svg+xml", DateTime.UtcNow.AddDays(1));
    }
}