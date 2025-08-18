// This is a view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
using System.Text.Json;

namespace Sefer.Backend.Api.Views.Admin.Users;

/// <summary>
/// This is a very extended view on the user.
/// It also contains user-type-specific details like mentor information.
/// </summary>
public class UserDetailsView(ExtendedUser data) : UserListView(data.User)
{
    /// <summary>
    /// In several sefer applications it is desirable to store more information on the user.
    /// Like country, location, but this is not needed for all applications, so this is build flexibility
    /// </summary>
    public Dictionary<string, JsonElement> AdditionalInfo => Model.AdditionalInfo;

    /// <summary>
    /// Returns is the user is considered to be active
    /// </summary>
    public bool IsActive => data.IsActive;

    /// <summary>
    /// The datetime the user was last active
    /// </summary>
    public DateTime? LastActive => data?.LastActivity?.ActivityDate;

    /// <summary>
    /// True when the user is a mentor else false
    /// </summary>
    public bool IsMentor => data.User.IsMentor;
    
    /// <summary>
    /// True when the user is a student else false
    /// </summary>
    public bool IsStudent => data.User.IsStudent;
    
    /// <summary>
    /// This data is set when the user is a mentor
    /// </summary>
    public MentorInfoView MentorInfo => IsMentor ? new MentorInfoView(data) : null;
    
    /// <summary>
    /// The class with all information on the mentor 
    /// </summary>
    public class MentorInfoView(ExtendedUser data)
    {
        /// <summary>
        /// Gets if the rating of the mentor
        /// </summary>
        public double? AverageRating => data?.MentorPerformance?.AverageRating;

        /// <summary>
        /// Gets the number of ratings this mentor has received 
        /// </summary>
        public int? RatingCount => data?.MentorPerformance?.RatingCount;

        /// <summary>
        /// Gets the number of active students for this mentor
        /// </summary>
        public long ActiveStudents => data.MentorActiveStudents ?? 0;

        /// <summary>
        /// Gets the preferred number of students for this mentor
        /// </summary>
        public int PreferredStudents => data.MentorSettings.PreferredStudents;

        /// <summary>
        /// Gets the maximum number of students for this mentor
        /// </summary>
        public int MaximumStudents => data.MentorSettings.MaximumStudents;
    }
}



