namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class GetBackupMentorHandler(IServiceProvider serviceProvider)
    : Handler<GetBackupMentorRequest, User>(serviceProvider)
{
    public override async Task<User> Handle(GetBackupMentorRequest request, CancellationToken token)
    {
        var settings = await Send(new GetSettingsRequest(), token);
        if (settings == null) return null;

        var backupMentor = await Send(new GetUserByIdRequest(settings.BackupMentorId), token);
        if (backupMentor == null || backupMentor.IsMentor == false) return null;
        return backupMentor;
    }
}