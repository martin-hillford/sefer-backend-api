// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NotAccessedField.Global
namespace Sefer.Backend.Api.Notifications.Mail.Models;

/// <summary>
/// Model for a sending an email about the enrollment of a student into a course
/// </summary>
public class VoucherRewardModel : MailModel
{
    /// <summary>
    /// The name of the student that is receiving the reward;
    /// </summary>
    public readonly string StudentName;

    /// <summary>
    /// The number of courses the student has completed
    /// </summary>
    public readonly int CoursesCompleted;

    /// <summary>
    /// The value of the voucher
    /// </summary>
    public readonly double VoucherValue;

    /// <summary>
    /// A verification code for the voucher
    /// </summary>
    public readonly string VoucherCode;

    /// <summary>
    /// The value of the voucher as a string
    /// </summary>
    public readonly string VoucherStringValue;

    /// <summary>
    /// Creates a new PasswordReset
    /// </summary>
    public VoucherRewardModel(MailData mailData, RewardGrant reward) : base(mailData)
    {
        StudentName = mailData.Receiver.Name;
        CoursesCompleted = reward.TargetReached;
        VoucherValue = reward.TargetValue ?? 0;
        VoucherCode = reward.Code;
        VoucherStringValue = VoucherValue.ToString("C2", mailData.GetCulture());
    }
}