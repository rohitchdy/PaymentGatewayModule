namespace PaymentGatewayAPI.Requests;

public record LoginRequest
(
    string UserName,
    string Password
);
