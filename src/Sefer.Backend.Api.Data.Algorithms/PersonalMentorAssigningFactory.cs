using Sefer.Backend.Api.Data.Requests.Mentors;
using Sefer.Backend.Api.Data.Requests.Settings;

namespace Sefer.Backend.Api.Data.Algorithms;

/// <summary>
/// This factory class prepares all the data for the personal mentor assigning algorithm.
/// </summary>
public class PersonalMentorAssigningFactory(IMediator mediator)
{
    public async Task<PersonalMentorAssigning> PrepareAlgorithmAsync(User student)
    {
        var settings = await mediator.Send(new GetSettingsRequest());
        var backupMentor = await mediator.Send(new GetBackupMentorRequest());
        var mentors = await mediator.Send(new GetMentorsWithSettingsRequest(UserRoles.Mentor));
        var studentsPerMentor = await mediator.Send(new GetMentorPersonalStudentsCountRequest());

        var input = new PersonalMentorAssigningInput
        {
            WebsiteSettings = settings,
            Mentors = mentors,
            Student = student,
            BackupMentor = backupMentor,
            StudentsPerMentor = studentsPerMentor
        };
        
        return new PersonalMentorAssigning(input);
    }
}