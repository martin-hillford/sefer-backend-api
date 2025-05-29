namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class SetMentorSettingsHandler(IServiceProvider serviceProvider)
    : UpdateEntityHandler<SetMentorSettingsRequest, MentorSettings>(serviceProvider);