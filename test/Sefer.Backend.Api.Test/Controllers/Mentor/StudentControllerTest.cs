using Sefer.Backend.Api.Controllers.Mentor;
using Sefer.Backend.Api.Services.ShortUrls;
using Sefer.Backend.Api.Views.Mentor;

namespace Sefer.Backend.Api.Test.Controllers.Mentor;

[TestClass]
public class StudentControllerTest : AbstractControllerTest
{
    [TestMethod]
    public async Task Test_CreatePersonalInvitation_NoMentor()
    {
        var user = new User { Role = UserRoles.Student, Id = 13 };
        var provider = GetServiceProvider(user);

        var controller = new StudentController(provider.Object);

        var result = await controller.CreatePersonalInvitation();

        result.Should().NotBeNull();
        result.Should().BeOfType<ForbidResult>();
    }

    [TestMethod]
    public async Task Test_CreatePersonalInvitation()
    {
        const string queryString = "hash=secret";
        var user = new User { Role = UserRoles.Mentor, Id = 13, PrimaryRegion = "nl", PrimarySite = "test.tld" };
        var cryptoService = new Mock<ICryptographyService>();

        var mentorSite = new Mock<ISite>();
        mentorSite.SetupGet(s => s.SiteUrl).Returns("https://test.tld");
        mentorSite.SetupGet(s => s.Hostname).Returns("test.tld");

        var mentorRegion = new Mock<IRegion>();
        mentorRegion.SetupGet(s => s.Id).Returns("nl");

        var fullUrl = $"{mentorSite.Object.SiteUrl}/register?{queryString}";
        var fallback = $"{mentorSite.Object.SiteUrl}/registration-expired";
        var provider = GetServiceProvider(user);
        var shortUrlService = new Mock<IShortUrlService>();

        cryptoService.Setup(c => c.TimeProtectedQueryString("pm", "13")).Returns(queryString);
        shortUrlService.Setup(c => c.Create(fullUrl, It.IsAny<DateTime>(), fallback)).ReturnsAsync(("shortUrl", "qrCode"));
        provider.AddService(cryptoService);
        provider.AddService(shortUrlService);
        provider.AddRequestResult<GetPrimaryRegionAndSiteRequest, (IRegion, ISite)>((mentorRegion.Object, mentorSite.Object));

        var controller = new StudentController(provider.Object);
        var result = await controller.CreatePersonalInvitation() as JsonResult;
        var value = result?.Value as PersonalInvitationView;

        result.Should().NotBeNull();
        value.Should().NotBeNull();
        value?.ShortUrl.Should().Be("shortUrl");
        value?.QRCode.Should().Be("qrCode");
        value?.FullUrl.Should().Be(fullUrl);
    }



}