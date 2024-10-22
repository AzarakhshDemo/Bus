namespace meTesting.Bus.SDK;

public static class UriBuilder
{   
    public static string BusUnSubscribe(this string baseUri, string key) => $"{baseUri}/Bus/UnSubscribe/{key}";
    public static string BusSubscribe(this string baseUri, string key) => $"{baseUri}/Bus/Subscribe/{key}";
    public static string BusPush(this string baseUri, string key) => $"{baseUri}/Bus/Push/{key}";
    public static string BusPull(this string baseUri, string key) => $"{baseUri}/Bus/Pull/{key}";
}
