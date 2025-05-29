// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Data.Handlers.Entities;

internal static class ContentBlockHandlerMethods
{
    private static List<T> GetByLesson<T>(DataContext context, int lessonId)
        where T : class, IContentBlock<Lesson, T>
    {
        var entitySet = context.Set<T>();
        return entitySet
            .Where(q => q.LessonId == lessonId)
            .OrderBy(q => q.SequenceId)
            .ToList();
    }

    internal static T GetById<T>(DataContext context, int id)
        where T : class, IContentBlock<Lesson, T>
    {
        var entitySet = context.Set<T>();
        return entitySet.SingleOrDefault(b => b.Id == id);
    }

    internal static bool Save<T>(IServiceProvider serviceProvider, DataContext context, T instance)
        where T : class, IContentBlock<Lesson, T>
    {
        return instance.Id < 1
            ? context.Insert(serviceProvider, instance)
            : context.Update(serviceProvider, instance);
    }

    internal static bool CreateSuccessor<T>(IServiceProvider serviceProvider, DataContext context, Lesson lesson, Lesson successorLesson)
        where T : class, IContentBlock<Lesson, T>
    {
        var blocks = GetByLesson<T>(context, lesson.Id);
        var successors = blocks.Select(b => b.CreateSuccessor(successorLesson));
        foreach (var successor in successors)
        {
            if (!context.Insert(serviceProvider, successor)) return false;
        }
        return true;
    }
}