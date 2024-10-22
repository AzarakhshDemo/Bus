using Microsoft.Extensions.Logging;

namespace meTesting.Bus.SDK;

public sealed class Subscriber(ILogger<Subscriber> logger, IOnRecieveEvent rec)
{

    internal event Func<Message, Task> OnReceive = rec.Do;

    public async Task Run(Message msg) => await OnReceive.Invoke(msg);
    public bool IsValid() => OnReceive is not null;
}
