namespace Sefer.Backend.Api.Data.Requests.ContentPages;

public class GetContentPageByIdRequest(int? id)
    : GetEntityByIdRequest<ContentPage>(id) { }