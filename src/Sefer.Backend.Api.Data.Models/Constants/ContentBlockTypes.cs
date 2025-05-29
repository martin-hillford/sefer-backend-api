namespace Sefer.Backend.Api.Data.Models.Constants;

/// <summary>
/// Definitions for all different element types.
/// </summary>
public enum ContentBlockTypes : short
{
    /// <summary>
    /// Text defines a pure text element
    /// </summary>
    ElementText = 100,

    /// <summary>
    /// Audio defines an audio element where the content property will be used as a caption
    /// </summary>
    ElementAudio = 101,

    /// <summary>
    /// Video defines a video element where the content property will be used as a caption
    /// </summary>
    ElementVideo = 102,

    /// <summary>
    /// YouTube defines a video element from YouTube where the content property will be
    /// used as a caption
    /// </summary>
    ElementYoutube = 103,

    /// <summary>
    /// Image defines an image element where the content property will be used as a caption
    /// </summary>
    ElementImage = 104,

    /// <summary>
    /// Link defines a link element (hyperlink to other websites)
    /// The content property will be used as the link text
    /// </summary>
    ElementLink = 105,

    /// <summary>
    /// Vimeo defines a video element Vimeo where the content property will be
    /// used as a caption
    /// </summary>
    ElementVimeo = 106,

    /// <summary>
    /// Defines an open question
    /// </summary>
    QuestionOpen = 200,

    /// <summary>
    /// Defines a boolean (yes/no , true/false) question
    /// </summary>
    QuestionBoolean = 201,

    /// <summary>
    /// Defines multiple choice question
    /// </summary>
    QuestionMultipleChoice = 202,
}