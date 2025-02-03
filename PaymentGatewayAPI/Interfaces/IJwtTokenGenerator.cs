using PaymentGatewayAPI.Models;

namespace PaymentGatewayAPI.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
