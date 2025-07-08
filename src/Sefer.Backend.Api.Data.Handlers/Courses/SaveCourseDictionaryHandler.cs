namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class SaveCourseDictionaryHandler(IServiceProvider serviceProvider) : Handler<SaveCourseDictionaryRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(SaveCourseDictionaryRequest request, CancellationToken token)
    {
        try
        {
            // Check if all the words of the request are from the same course revision.
            var revisionId = request.CourseRevisionId;
            var revisions = request.Words.Values.Select(w => w.CourseRevisionId).Distinct().ToList();
            
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (revisions.Count > 1) return false;
            if(revisions.Count == 1 && revisions.First() != request.CourseRevisionId) return false;
            
            // Check if the courseRevision to add exists and if that is editable
            await using var context = GetDataContext();
            var revision =  await context.CourseRevisions.SingleOrDefaultAsync(r => r.Id == revisionId, token);
            if(revision is not { IsEditable: true }) return false;
        
            // Create a dictionary with all the existing words in the database
            var current = await context.CourseRevisionDictionaryWords
                .Where(w => w.CourseRevisionId == revisionId).ToDictionaryAsync(t => t.Word, t => t, token);
        
            // Check for each word if it already exists or if it should be updated
            foreach (var (key, word) in request.Words)
            {
                var exists = current.TryGetValue(key, out var existing);
                if (!exists) context.CourseRevisionDictionaryWords.Add(word);
                else
                {
                    existing.Explanation = word.Explanation;
                    existing.Language = word.Language;
                    existing.PictureUrl = word.PictureUrl;
                }
            }
        
            // Also check for each existing word if it should be deleted
            var deleted = current.Where(w => !request.Words.ContainsKey(w.Key)).Select(c => c.Value).ToList();
            context.CourseRevisionDictionaryWords.RemoveRange(deleted);
        
            // And save everything
            await context.SaveChangesAsync(token);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}