namespace Sefer.Backend.Api.Data.Requests.Channels;

public class GetChannelByIdRequest(int? id) : GetEntityByIdRequest<Channel>(id);