using meTesting.Bus.SDK;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace meTesting.Bus.Controllers;


[ApiController]
[Route("[controller]")]
public class BusController : ControllerBase
{
    private static ConcurrentDictionary<string, ConcurrentQueue<Message>> queue = new();

    private readonly ILogger<BusController> _logger;

    public BusController(ILogger<BusController> logger)
    {
        _logger = logger;
        if (queue.IsEmpty)
            Subscribe("m").Wait();
    }

    [HttpGet("[action]/{sub}")]
    public async Task Subscribe(string sub)
    {
        queue[sub] = new ConcurrentQueue<Message>();
    }
    [HttpGet("[action]/{sub}")]
    public async Task UnSubscribe(string sub)
    {
        queue.Remove(sub, out _);
    }
    [HttpPost("[action]/{sender}")]
    public async Task Push(string sender, [FromBody] Message val)
    {
        foreach (var q in queue.Where(a => a.Key != sender))
        {
            q.Value.Enqueue(val);
        };
    }

    [HttpGet("[action]/{receiver}")]
    public async Task<Message> Pull(string receiver)
    {
        queue[receiver].TryDequeue(out var message);
        return message;
    }

}

