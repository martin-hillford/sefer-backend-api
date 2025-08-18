using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sefer.Backend.Api.Shared.Test;

[TestClass]
public class DefaultJsonOptionsTest
{
    [TestMethod]
    public void GetOptionsNotNull()
    {
        var options = DefaultJsonOptions.GetOptions();
        Assert.IsNotNull(options);
        AsserOptions(options);
    }

    [TestMethod]
    public void SetOptionsTest()
    {
        var options = new JsonSerializerOptions();
        DefaultJsonOptions.SetOptions(options);
        AsserOptions(options);
    }
    
    private static void AsserOptions(JsonSerializerOptions options)
    {
        Assert.IsTrue(options.AllowTrailingCommas);
        Assert.IsFalse(options.IgnoreReadOnlyFields);
        Assert.IsFalse(options.IgnoreReadOnlyProperties);
        Assert.IsTrue(options.PropertyNameCaseInsensitive);
        Assert.IsTrue(options.IncludeFields);
        Assert.IsFalse(options.WriteIndented);
        Assert.AreEqual(JsonNumberHandling.AllowReadingFromString, options.NumberHandling);
        Assert.AreEqual(JsonNamingPolicy.CamelCase, options.PropertyNamingPolicy);
        Assert.AreEqual(JsonCommentHandling.Skip, options.ReadCommentHandling);
        Assert.AreEqual(JsonNamingPolicy.CamelCase, options.DictionaryKeyPolicy);
    }
}
