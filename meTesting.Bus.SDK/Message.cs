namespace meTesting.Bus.SDK;

public class Message
{
    public string Body { get; set; }
    public DateTime TimeStamp { get; set; }
    public string Sender { get; set; }
    public string? Type { get; set; }
    public string? AggregateId { get; set; }
}
