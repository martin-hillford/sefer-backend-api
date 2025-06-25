namespace Sefer.Backend.Api.Views.Admin.Resources;

public class TemplateView(IGrouping<string, Template> group)
{
    public readonly string Name = group.Key;
    
    public readonly ReadOnlyCollection<LanguageView> Languages
        = group.GroupBy(g => g.Language).Select(g => new LanguageView(g)).ToList().AsReadOnly();
}       

public class LanguageView(IGrouping<string, Template> group)
{
    public readonly string Language = group.Key;
    
    public readonly string Title = group.FirstOrDefault()?.Title;

    public readonly Template Html = group.FirstOrDefault(g => g.Type == "html");
    
    public readonly Template Text = group.FirstOrDefault(g => g.Type == "text"); 
}