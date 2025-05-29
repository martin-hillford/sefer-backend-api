namespace Sefer.Backend.Api.Data.Handlers.Courses;

internal class CoursePrerequisiteHelper : HelperClass
{
    internal CoursePrerequisiteHelper(IServiceProvider serviceProvider, DataContext context)
        : base(serviceProvider, context) { }

    internal bool CreatesLoop(CoursePrerequisite prerequisite)
    {
        var required = GetRequiredCoursesRecursive(prerequisite.RequiredCourseId);
        return required.Any(c => c.Id == prerequisite.CourseId);
    }

    private List<Course> GetRequiredCoursesRecursive(int courseId)
    {
        // Load all the information from the database
        var requisites = Context.CoursePrerequisites
            .Include(r => r.Course)
            .Include(r => r.RequiredCourse)
            .ToList();

        // create the proper lookup table
        Course course = null;
        var lookup = new Dictionary<Course, HashSet<Course>>();
        foreach (var req in requisites)
        {
            if (lookup.ContainsKey(req.Course) == false) lookup.Add(req.Course, new HashSet<Course>());
            if (lookup[req.Course].Contains(req.RequiredCourse) == false) lookup[req.Course].Add(req.RequiredCourse);
            if (req.CourseId == courseId) course = req.Course;
            if (req.RequiredCourseId == courseId) course = req.RequiredCourse;
        }

        // do a recursive retrieval
        if (course == null) return new List<Course>();
        var required = GetRequiredCoursesRecursive(lookup, new HashSet<Course>(), course);

        // Sort and return
        return required.OrderBy(c => c.Name).ToList();
    }

    private static HashSet<Course> GetRequiredCoursesRecursive(IReadOnlyDictionary<Course, HashSet<Course>> lookup, HashSet<Course> courses, Course course)
    {
        if (lookup.TryGetValue(course, out var value) == false) return courses;
        foreach (var required in value)
        {
            if (courses.Contains(required)) continue;
            var deep = GetRequiredCoursesRecursive(lookup, courses, required);
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var elem in deep)
            {
                if (courses.Contains(required) == false) courses.Add(elem);
            }
            courses.Add(required);
        }

        return courses;
    }

    internal async Task<bool> Add(CoursePrerequisite entity)
    {
        if (entity.CourseId == entity.RequiredCourseId) return false;
        var course = Context.Courses.SingleOrDefault(c => c.Id == entity.CourseId);
        var required = await Context.Courses.SingleOrDefaultAsync(c => c.Id == entity.RequiredCourseId);
        if (course == null || required == null) return false;

        if (CreatesLoop(entity)) return false;

        var exists = Context.CoursePrerequisites.Any(r => r.CourseId == course.Id && r.RequiredCourseId == required.Id);
        return exists || Context.Insert(ServiceProvider, entity);
    }

    internal bool Delete(CoursePrerequisite entity)
    {
        var existing = Context.CoursePrerequisites
            .Where(r => r.CourseId == entity.CourseId && r.RequiredCourseId == entity.RequiredCourseId)
            .ToList();

        Context.CoursePrerequisites.RemoveRange(existing);
        Context.SaveChanges();
        return true;
    }
}