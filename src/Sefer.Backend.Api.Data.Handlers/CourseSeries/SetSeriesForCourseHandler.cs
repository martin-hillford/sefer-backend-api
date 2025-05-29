namespace Sefer.Backend.Api.Data.Handlers.CourseSeries;

public class SetSeriesForCourseHandler(IServiceProvider serviceProvider)
    : SyncHandler<SetSeriesForCourseRequest, bool>(serviceProvider)
{
    protected override bool Handle(SetSeriesForCourseRequest request)
    {
        // check for null
        var context = GetDataContext();
        var series = context.Series.SingleOrDefault(c => c.Id == request.SeriesId);
        if (series == null || request.Courses == null) return false;

        // Create a lookup table for the post courses with the sequence id as value also load the existing relations
        var lookup = request.Courses.Select((c, i) => new { Course = c, Index = i }).ToDictionary(c => c.Course.Id, c => c.Index);
        var existing = context.SeriesCourses.Where(sr => sr.SeriesId == series.Id).ToDictionary(c => c.CourseId);

        // Now loop through the existing and check if an update or delete is needed
        foreach (var seriesCourse in existing.Values)
        {
            if (lookup.TryGetValue(seriesCourse.CourseId, out var value)) seriesCourse.SequenceId = value;
            else context.SeriesCourses.Remove(seriesCourse);
        }

        // Now loop through the list and insert the new ones
        foreach (var (courseId, sequence) in lookup)
        {
            if (existing.ContainsKey(courseId)) continue;
            var seriesCourse = new SeriesCourse(series.Id, courseId, sequence);
            context.SeriesCourses.Add(seriesCourse);
        }

        // Save the saves and return
        context.SaveChanges();
        return true;
    }
}