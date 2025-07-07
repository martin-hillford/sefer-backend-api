// As this is a view, the get method of the properties may only be used by the JSON serialization
// ReSharper disable MemberCanBePrivate.Global, UnusedMember.Global

using Sefer.Backend.Api.Models.Public;

namespace Sefer.Backend.Api.Views.Public.Download;

public class Choice(MultipleChoiceQuestionChoice choice)
{
    public string Answer { get; set; } = choice.Answer;
    
    public bool IsCorrectAnswer => choice.IsCorrectAnswer;

    public int Id => choice.Id;
    
    public async Task IncludeMedia(DownloadRequest request, Course course)
    {
        var images = ContentSupport.FindImageUrls(Answer);
        foreach (var imageUrl in images)
        {
            var resource = await ContentSupport.CreateResource(request, imageUrl);
            if (resource == null) continue;
            course.Resources.Add(resource);
            Answer = Answer.Replace(imageUrl, resource.GetResourceUrl());
        }
    }
}