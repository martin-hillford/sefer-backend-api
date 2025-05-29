namespace Sefer.Backend.Api.Data.Models.Abstractions;

/// <summary>
/// Defines an interface for question of lesson (basically a combination of a lesson and a content block)
/// </summary>
/// <typeparam name="TLesson"></typeparam>
/// <typeparam name="TQuestion"></typeparam>
public interface ILessonQuestion<TLesson, out TQuestion> :  IContentBlock<TLesson, TQuestion>, IQuestion
    where TLesson : ILesson
    where TQuestion : ILessonQuestion<TLesson, TQuestion> 
{ }