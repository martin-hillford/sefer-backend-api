namespace Sefer.Backend.Api.Data.Models.Constants;

/// <summary>
/// Defines the types of messages that are to be sent in the system
/// </summary>
public enum MessageTypes : short
{
    /// <summary>
    /// This defines a message in which the lesson is submitted from the student to mentor
    /// </summary>
    StudentLessonSubmission = 1,

    /// <summary>
    /// This defines a message in which is a chat message from one to another user
    /// </summary>
    Text = 2,

    /// <summary>
    /// This defines a message in which the lesson is submitted from the student to mentor
    /// </summary>
    NameChange = 3,

    /// <summary>
    /// This defines a message in which the lesson submission is reviewed by the mentor to the student
    /// </summary>
    MentorLessonSubmissionReview = 4,

    /// <summary>
    /// This defines a message which tells that a student signed up
    /// </summary>
    StudentEnrollment = 5,

    /// <summary>
    /// This defines a message that is a review of an answer
    /// </summary>
    MentorAnswerReview = 6,

    /// <summary>
    /// This defines that a student has left the mentor and changed to another
    /// </summary>
    MentorChangeLeave = 7,

    /// <summary>
    /// This defines that a student has left other mentor and changed to this mentor
    /// </summary>
    MentorChangeEnter = 8,
}