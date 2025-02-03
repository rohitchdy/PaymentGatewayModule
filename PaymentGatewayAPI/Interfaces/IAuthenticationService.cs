using PaymentGatewayAPI.Models;

namespace PaymentGatewayAPI.Interfaces;

public interface IAuthenticationService
{
    Task<User>? GetUserByUserName(string userName);
}
