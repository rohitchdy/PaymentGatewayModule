namespace PaymentGatewayAPI.Models;

public class Transaction
{
    public Guid TransactionId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string PaymentMode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public required Customer Customer { get; set; }
}
