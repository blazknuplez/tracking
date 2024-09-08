using MassTransit;
using Microsoft.Extensions.Options;
using Tracking.Client.Options;
using Tracking.Contracts.Events;

namespace Tracking.Client.Filters;

public class AccountFilter :  IFilter<ConsumeContext<TrackingEventReceived>>
{
    private readonly AccountFilterOptions _options;

    public AccountFilter(IOptions<AccountFilterOptions> options)
    {
        _options = options.Value;
    }

    public async Task Send(ConsumeContext<TrackingEventReceived> context,
        IPipe<ConsumeContext<TrackingEventReceived>> next)
    {
        if (ShouldSendMessage(context.Message))
        {
            await next.Send(context);
        }
    }

    public void Probe(ProbeContext context)
    {
    }

    private bool ShouldSendMessage(TrackingEventReceived message)
    {
        return !_options.IsFilteringEnabled || _options.AccountIds.Contains(message.AccountId);
    }
}