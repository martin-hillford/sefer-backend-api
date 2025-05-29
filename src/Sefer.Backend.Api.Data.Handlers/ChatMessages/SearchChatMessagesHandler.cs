using System.Text.RegularExpressions;

namespace Sefer.Backend.Api.Data.Handlers.ChatMessages;

public partial class SearchChatMessagesHandler(IServiceProvider serviceProvider)
    : Handler<SearchChatMessagesRequest, List<ChatSearchResult>>(serviceProvider)
{
    [GeneratedRegex("[^a-zA-Z']")]
    private static partial Regex CleanRegex();
    
    private static readonly Regex Clean = CleanRegex();

    public override async Task<List<ChatSearchResult>> Handle(SearchChatMessagesRequest request, CancellationToken token)
    {
        try
        {
            if (string.IsNullOrEmpty(request.SearchTerm)) return [];
            var term = Clean.Replace(request.SearchTerm, string.Empty);
            if (string.IsNullOrEmpty(term)) return [];

            var context = GetDataContext();
            var query = context.Database.IsSqlServer()
                ? context.ChatMessages.Where(m => EF.Functions.Contains(m.ContentString, $"\"{term}*\""))
                : context.ChatMessages.Where(m => m.ContentString.Contains(request.SearchTerm));

            return await query
                .Where(m => m.Type == MessageTypes.Text && m.ChannelMessages.Any(c => c.ReceiverId == request.UserId))
                .Select(m => new ChatSearchResult { ChannelId = m.ChannelId, MessageId = m.Id, Content = m.ContentString })
                .ToListAsync(cancellationToken: token);
        }
        catch (Exception) { return new(); }
    }

    
}