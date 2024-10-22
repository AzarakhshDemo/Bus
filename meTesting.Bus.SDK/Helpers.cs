using Microsoft.Extensions.DependencyInjection;
using meTesting.Sauron;

namespace meTesting.Bus.SDK;

public static class Helpers
{
    public static IServiceCollection AddServiceBus<T>(this IServiceCollection services,
        Action<AzBusConfig> config,
        Func<IServiceProvider, T> handlerFactory) where T : class, IOnRecieveEvent
    {
        var busConfig = new AzBusConfig();
        config(busConfig);

        services.AddSingleton(busConfig);

        

        services.AddSingleton<Subscriber>();
        services.AddSingleton<Publisher>();

        services.AddSauronHttpClient<SubscribeLoopService>();
        services.AddSauronHttpClient<Publisher>();

        services.AddHostedService<SubscribeLoopService>();

        services.AddSingleton<IOnRecieveEvent>(handlerFactory);

        return services;
    }
}
