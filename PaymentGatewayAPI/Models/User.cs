﻿namespace PaymentGatewayAPI.Models;

public class User
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public List<Transaction>? Transactions { get; set; }
}
