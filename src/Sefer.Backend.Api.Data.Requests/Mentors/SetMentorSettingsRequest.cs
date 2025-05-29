namespace Sefer.Backend.Api.Data.Requests.Mentors;

public class SetMentorSettingsRequest(MentorSettings entity) : UpdateEntityRequest<MentorSettings>(entity);