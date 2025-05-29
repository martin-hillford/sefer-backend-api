using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Message = FirebaseAdmin.Messaging.Message;

namespace Sefer.Backend.Api.Notifications.Push;

public class FireBase : IFireBase
{
    private static bool _created;

    private readonly IMediator _mediator;

    public FireBase(IServiceProvider serviceProvider, ILogger<FireBase> logger)
    {
        _mediator = serviceProvider.GetService<IMediator>();
        if (_created) return;

        try
        {
            var options = serviceProvider.GetService<IOptions<FireBaseOptions>>();
            var credentials = GoogleCredential.FromFile(options.Value.KeyFile);
            FirebaseApp.Create(new AppOptions { Credential = credentials });

            _created = true;

            logger.LogInformation("Firebase service started");
        }
        catch (Exception exception) { logger.LogError(exception, "Error firebase"); }
    }

    public async Task<List<Exception>> SendMessage(int userId, string title, string body)
    {
        var tokens = await _mediator.Send(new GetPushNotificationTokensByUserIdRequest(userId));
        var messages = tokens.Select(token => CreateMessage(title, body, token));
        var exceptions = new List<Exception>();
        foreach (var message in messages)
        {
            try { await FirebaseMessaging.DefaultInstance.SendAsync(message); }
            catch (Exception exception) { exceptions.Add(exception); }
        }
        return exceptions;
    }

    private static Message CreateMessage(string title, string body, string token)
    {
        return new Message
        {
            Notification = new Notification { Title = title, Body = body },
            Token = token,
            Android = new AndroidConfig
            {
                Notification = new AndroidNotification
                {
                    DefaultSound = true,
                    Title = title,
                    Body = body,
                    DefaultVibrateTimings = true,
                    LocalOnly = false,
                }
            }
        };
    }
}