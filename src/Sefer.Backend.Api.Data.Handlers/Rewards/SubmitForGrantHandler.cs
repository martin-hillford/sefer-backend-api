using Microsoft.Extensions.Configuration;

namespace Sefer.Backend.Api.Data.Handlers.Rewards;

public class SubmitForGrantHandler(IServiceProvider serviceProvider)
    : Handler<SubmitForGrantRequest, List<RewardGrant>>(serviceProvider)
{
    public override async Task<List<RewardGrant>> Handle(SubmitForGrantRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var enrollment = await context.Enrollments.SingleOrDefaultAsync(e => e.Id == request.EnrollmentId, token);
        if (enrollment == null) return null;

        var config = ServiceProvider.GetService<IConfiguration>();
        var processors = await RewardProcessorFactory.GetProcessors(Mediator, config);
        var tasks = processors.Select(processor => processor.Process(enrollment));
        var rewards = await Task.WhenAll(tasks);
        return rewards.SelectMany(r => r).ToList();
    }
}