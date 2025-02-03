namespace PaymentGatewayAPI.Models;

public class Customer
{
    public Guid CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public Guid UserId { get; set; }
    public required User User { get; set; }
    public List<Transaction>? Transactions { get; set; }
}
