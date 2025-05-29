namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class GetMediaElementByIdRequest(int? id) : GetEntityByIdRequest<MediaElement>(id);