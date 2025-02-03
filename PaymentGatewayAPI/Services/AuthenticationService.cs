using Microsoft.EntityFrameworkCore;
using PaymentGatewayAPI.DatabaseContext;
using PaymentGatewayAPI.Interfaces;
using PaymentGatewayAPI.Models;

namespace PaymentGatewayAPI.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly ApplicationDbContext _context;
    public AuthenticationService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<User?> GetUserByUserName(string userName)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
    }
}
