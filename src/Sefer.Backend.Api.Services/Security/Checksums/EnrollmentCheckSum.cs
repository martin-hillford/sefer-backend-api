namespace Sefer.Backend.Api.Services.Security.Checksums;

/// <summary>
/// This class helps with checksum for enrollments
/// </summary>
public static class EnrollmentChecksum
{
    /// <summary>
    /// This is a xor value for the int, to make it obvious it are increasing numbers
    /// </summary>
    private const int XorValue = 0b01111101000110011011101010100110;

    /// <summary>
    /// This method will generate a checksum based on the enrollment
    /// </summary>
    /// <param name="enrollment">The enrollment to generate the checksum for</param>
    /// <returns></returns>
    public static string GetChecksum(Enrollment enrollment)
    {
        if (enrollment == null) throw new ArgumentException("Please provide an enrollment", nameof(enrollment));
        var value = enrollment.Id ^ XorValue;
        return value.ToString("X").ToUpper();
    }

    /// <summary>
    /// The checksum to retrieve the enrollment for
    /// </summary>
    /// <param name="checksum"></param>
    public static int? GetEnrollmentId(string checksum)
    {
        try
        {
            var value = Convert.ToInt32(checksum, 16);
            var enrollmentId = value ^ XorValue;
            return enrollmentId;
        }
        catch (Exception) { return null; }
    }
}