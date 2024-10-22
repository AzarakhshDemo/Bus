namespace meTesting.Bus.SDK;

public interface IOnRecieveEvent
{
    Task Do(Message message);
}

