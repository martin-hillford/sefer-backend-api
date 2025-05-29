// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
namespace Sefer.Backend.Api.Views.Public.Users;

public sealed class RequestRegionView(IRegion selected, IEnumerable<IRegion> regions)
{
    public string Selected => selected.Id;

    public IEnumerable<IRegion> Regions => regions;
}