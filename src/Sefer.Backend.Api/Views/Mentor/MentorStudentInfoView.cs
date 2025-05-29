// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
using Sefer.Backend.Api.Data.JsonViews;
using Sefer.Backend.Api.Services.Avatars;

namespace Sefer.Backend.Api.Views.Mentor;

/// <summary>
/// The view of the mentor on the student
/// </summary>
public class MentorStudentInfoView : AbstractView<User>
{
    /// <summary>
    /// Gets the name of the user
    /// </summary>
    public string Name => Model.Name;

    /// <summary>
    /// Gets or sets the gender of user
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Genders Gender => Model.Gender;

    /// <summary>
    /// Gets or sets the year the user was born. Useful to determine (together with gender)
    /// the mentor to be assigned to the user if he is student
    /// </summary>
    public short YearOfBirth => Model.YearOfBirth;

    /// <summary>
    /// Gets or sets some personal information provided by users about himself
    /// </summary>
    public string Info => Model.Info;

    /// <summary>
    /// Returns the date the student has subscribed into his account
    /// </summary>
    public DateTime SubscriptionDate => Model.SubscriptionDate;

    /// <summary>
    /// Return if the student is blocked
    /// </summary>
    public bool Blocked => Model.Blocked;

    /// <summary>
    /// Hold any remarks the mentor has added to the student
    /// </summary>
    public readonly string Remarks;

    /// <summary>
    /// The avatar url for the student
    /// </summary>
    public readonly string AvatarUrl;

    /// <summary>
    /// Creates the view of the mentor on the student
    /// </summary>
    public MentorStudentInfoView(User student, MentorStudentData data, IAvatarService avatarService) : base(student)
    {
        Remarks = data?.Remarks ?? string.Empty;
        AvatarUrl = avatarService.GetAvatarUrl(Id, Name);
    }
}