using PaymentGatewayAPI.Models;

namespace PaymentGatewayAPI.Requests;

public record AuthenticationResponse(
    User user,
    string Token
);
