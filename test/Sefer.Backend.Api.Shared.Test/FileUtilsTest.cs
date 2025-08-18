namespace Sefer.Backend.Api.Shared.Test;

[TestClass]
public class FileUtilsTest
{
    [TestMethod]
    public void GetTemporaryDirectory()
    {
        var directory = FileUtils.GetTemporaryDirectory();
        var exists = Directory.Exists(directory);
        Assert.IsNotNull(directory);
        Assert.IsTrue(exists);
        Directory.Delete(directory, true);
    }
    
    [TestMethod]
    public void GetTemporaryDirectoryWithName()
    {
        var directory = FileUtils.GetTemporaryDirectory("temp-name");
        var exists = Directory.Exists(directory);
        Assert.IsNotNull(directory);
        Assert.IsTrue(exists);
        Assert.IsTrue(directory.EndsWith("temp-name"));
        Directory.Delete(directory, true);
    }

    [TestMethod]
    public void ToStream()
    {
        const string data = "This is a test.";
        using var stream = data.ToStream();
        using var reader = new StreamReader(stream);
        stream.Position = 0;
        var actual = reader.ReadToEnd();
        Assert.AreEqual(data, actual);
    }
}

