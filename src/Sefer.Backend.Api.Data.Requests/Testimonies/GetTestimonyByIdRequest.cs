namespace Sefer.Backend.Api.Data.Requests.Testimonies;

public class GetTestimonyByIdRequest(int? id) : GetEntityByIdRequest<Testimony>(id);