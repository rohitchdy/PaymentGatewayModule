namespace PaymentGatewayAPI.Configurations;

public class RabbitMQConfiguration
{
    public const string SectionName = "RabbitMQSettings";
    public string HostName { get; init; } = null!;
    public int PortNo { get; init; }
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
