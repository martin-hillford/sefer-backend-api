namespace Sefer.Backend.Api.Shared.Test.Cryptography;

[TestClass]
public class HashingTest
{
    [TestMethod]
    public void Sha512FromString()
    {
        const string data = "This is a hashing test.";
        const string expected = "Bs58yBEdzm1USlhDBTQ7o+2nAIfCvvd/2EpcRTNWWqtoQGgFJDxWT0EPzSDCnMm6Ojh1KCLPn5DEf7KO/FYWQQ==";
        
        var actual = Shared.Cryptography.Hashing.Sha512(data);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void HashPasswordNotEmpty()
    {
        const string password = "This is a hashing test.";
        const string salt = "2EpcRTNWWqtoQGgFJDxWT0EPzSDCnMm6Ojh1KCLPn5DEf7KO";
        
        var actual = Shared.Cryptography.Hashing.HashPassword(password, salt);
        Assert.IsNotNull(actual); Assert.AreNotEqual(string.Empty, actual);
    }
}