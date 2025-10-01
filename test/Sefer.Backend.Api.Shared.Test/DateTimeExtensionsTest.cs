namespace Sefer.Backend.Api.Shared.Test;

[TestClass]
public class DateTimeExtensionsTest
{
    [TestMethod]
    public void ToPostgresTimestamp()
    {
        var dateTime = new DateTime(2019, 9, 7, 14, 59, 30, 262);
        var actual = dateTime.ToUniversalTime().ToPostgresTimestamp();
        // Nb. this is not the format used in postgres itself,
        //     but this is a format postgres correctly processes.  
        const string expected = "2019-09-07T12:59:30.2620000+00:00";  
        Assert.AreEqual(expected, actual);
    }
}

