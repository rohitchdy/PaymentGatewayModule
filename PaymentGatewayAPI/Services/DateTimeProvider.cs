using PaymentGatewayAPI.Interfaces;

namespace PaymentGatewayAPI.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
