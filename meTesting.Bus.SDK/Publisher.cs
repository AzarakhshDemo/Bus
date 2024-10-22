using meTesting.Sauron;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace meTesting.Bus.SDK;

public class Publisher(
    ILogger<Publisher> logger,
    HttpClient httpClient,
    AzBusConfig config)

{
    public ILogger<Publisher> Logger { get; }

    public async void Publish<T>(T val)
    {

        var pubm = new Message()
        {
            TimeStamp = DateTime.Now,
            Body = JsonSerializer.Serialize(val),
            Type = typeof(T).Name,
            Sender = config.Key,
            AggregateId = httpClient.DefaultRequestHeaders.GetAggregateId(out var id) ? id : default
        };

        var res = await httpClient.PostAsJsonAsync(new Uri(config.BaseUrl.BusPush(config.Key)), pubm);

        if (!res.IsSuccessStatusCode)
            throw new Exception($"Push messgae encountered an error: {await res.Content.ReadAsStringAsync()}");
    }
}
