using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Sefer.Backend.Api.Shared.Validation;
using Moq;

// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Sefer.Backend.Api.Shared.Test.Validation;

[TestClass]
public class JsonDictionaryAttributeTest
{
    [TestMethod]
    public void ObjectNull()
    {
        var data = new ValidatingObject();
        var context = CreateContext(data, []);
        
        
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(data, context, results, true);
        
        Assert.IsTrue(isValid);
        Assert.AreEqual(0, results.Count);
    }
    
    [TestMethod]
    public void ObjectEmpty()
    {
        var data = new ValidatingObject { AdditionalElements = [] };
        var context = CreateContext(data, []);
        
        
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(data, context, results, true);
        
        Assert.IsTrue(isValid);
        Assert.AreEqual(0, results.Count);
    }
    
    [TestMethod]
    public void Valid()
    {
        var keys = new List<string> {"key1", "key2"};
        var elements = new Dictionary<string, JsonElement> { { "key1", JsonDocument.Parse("\"Alice\"").RootElement.Clone() } };
        var data = new ValidatingObject { AdditionalElements = elements };
        var context = CreateContext(data, keys);
        
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(data, context, results, true);
        
        Assert.IsTrue(isValid);
        Assert.AreEqual(0, results.Count);
    }
    
    [TestMethod]
    public void InValidKey()
    {
        var keys = new List<string> {"key1", "key2"};
        var elements = new Dictionary<string, JsonElement> { { "name", JsonDocument.Parse("\"Alice\"").RootElement.Clone() } };
        var data = new ValidatingObject { AdditionalElements = elements };
        var context = CreateContext(data, keys);
        
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(data, context, results, true);
        
        Assert.IsFalse(isValid);
        Assert.AreEqual(1, results.Count);
        Assert.AreEqual("Not all keys are not allowed", results[0].ErrorMessage);
    }
    
    [TestMethod]
    public void TooLong()
    {
        var keys = new List<string> {"key1", "key2"};
        var elements = new Dictionary<string, JsonElement> { { "key1", JsonDocument.Parse("\"Alice\"").RootElement.Clone() } };
        var data = new MinLengthValidatingObject { AdditionalElements = elements };
        var context = CreateContext(data, keys);
        
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(data, context, results, true);
        
        Assert.IsFalse(isValid);
        Assert.AreEqual(1, results.Count);
        Assert.AreEqual("To much data stored in object, maxSize 10", results[0].ErrorMessage);
    }
    
    [TestMethod]
    public void InValidType()
    {

        var data = new InValidTypeObject { AdditionalElements = [1, 2] };
        var context = CreateContext(data, []);
        
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(data, context, results, true);
        
        Assert.IsFalse(isValid);
        Assert.AreEqual(1, results.Count);
        Assert.AreEqual("The value is not a dictionary", results[0].ErrorMessage);
    }
    
    private static ValidationContext CreateContext(object data, List<string> allowedKeys)
    {
        var keyDictionary = Enumerable.Range(0, allowedKeys.Count)
            .ToDictionary(i => $"testKeys:{i}", i => allowedKeys[i])
            .Select(kvp => new KeyValuePair<string, string?>(kvp.Key, kvp.Value));
        var builder = new ConfigurationBuilder().AddInMemoryCollection(keyDictionary);
        var configuration = builder.Build();
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(s => s.GetService(typeof(IConfiguration))).Returns(configuration);
        return new ValidationContext(data, serviceProvider.Object, null);
    }

    private class ValidatingObject
    {
        [JsonDictionary("testKeys")]
        public Dictionary<string, JsonElement>? AdditionalElements { get; set; }
    }
    
    private class MinLengthValidatingObject
    {
        [JsonDictionary("testKeys", 10)]
        public Dictionary<string, JsonElement>? AdditionalElements { get; set; }
    }
    
    private class InValidTypeObject
    {
        [JsonDictionary("testKeys")]
        public List<int>? AdditionalElements { get; set; }
    }
}