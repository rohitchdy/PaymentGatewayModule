﻿namespace PaymentGatewayAPI.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
