namespace Sefer.Backend.Api.Data.Validation;

/// <summary>
/// This method helps in converting a 'boolean' string to a real string
/// </summary>
public static class BooleanStringConvertor
{
    /// <summary>
    /// This method convert a boolean string to an actual boolean
    /// </summary>
    public static bool? Convert(string boolean)
    {
        if (string.IsNullOrEmpty(boolean)) return null;
        return boolean.ToLower() switch
        {
            "correct" or "true" or "1" => true,
            "wrong" or "false" or "0" => false,
            _ => null
        };
    }
}