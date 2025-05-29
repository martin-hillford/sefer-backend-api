namespace Sefer.Backend.Api.Notifications.Test.Push;

[TestClass]
public class FireBaseServiceTest
{

    [TestMethod]
    public void ReplaceVars_Test()
    {
        var content = ( Title: "@r @s @t", Body: "a@r a@s x");
        var vars = new Dictionary<string, string> { { "@r", "r" }, { "@s", "s" }, { "@t", "t" } };
        var ( title, body ) = FireBaseService.ReplaceVars(content, vars);
        
        Assert.AreEqual("r s t", title );
        Assert.AreEqual("ar as x", body );
    }
}