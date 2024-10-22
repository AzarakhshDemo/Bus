using meTesting.Sauron;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Context;
using System.Text.Json;

namespace meTesting.Bus.SDK;

class SubscribeLoopService : IHostedService
{
    private readonly ILogger<SubscribeLoopService> logger;
    private readonly Subscriber sub;
    private readonly AzBusConfig config;
    private readonly HttpClient client;

    private bool isSub = false;

    public SubscribeLoopService(ILogger<SubscribeLoopService> logger,
        AzBusConfig opt,
        Subscriber sub)
    {
        this.logger = logger;
        this.sub = sub;
        this.config = opt;
        client = new HttpClient();
        client.BaseAddress = new Uri(opt.BaseUrl);

        if (!sub.IsValid())
            throw new Exception("no handler for OnReceive event is found");

    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () => await Loop(cancellationToken));
    }

    private async Task Loop(CancellationToken cancellationToken)
    {
        while (true)
        {
            Message? msg = default;
            try
            {
                if (!isSub)
                    isSub = (await client.GetAsync(config.BaseUrl.BusSubscribe(config.Key))).IsSuccessStatusCode;

                var res = await client.GetAsync(config.BaseUrl.BusPull(config.Key));
                if (!res.IsSuccessStatusCode)
                    continue;

                var resp = await res.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(resp)) continue;

                msg = JsonSerializer.Deserialize<Message>(resp, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (msg is not null)
                    using (LogContext.PushProperty(SauronConstants.AGGREGATE_ID_KEY, msg.AggregateId))
                    {
                        await sub.Run(msg);
                    }

                if (false || cancellationToken.IsCancellationRequested)
                    break;
            }
            catch (Exception ex)
            {
                using (LogContext.PushProperty(SauronConstants.AGGREGATE_ID_KEY, msg?.AggregateId))
                    logger.LogError(ex, $"{this.GetType().Name} has error: {ex.Message}");
            }
            finally
            {
                await Task.Delay(1000);
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await client.GetAsync(config.BaseUrl.BusUnSubscribe(config.Key));

    }
}
