using Microsoft.EntityFrameworkCore;
using PaymentGatewayAPI.DatabaseContext;
using PaymentGatewayAPI.Interfaces;
using PaymentGatewayAPI.Models;
using System.Security.Claims;

namespace PaymentGatewayAPI.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Guid?> GetCurrentUserId()
    {
        var userIdString = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdString, out var userId))
        {
            return userId;
        }
        return null;
    }

    public async Task<User?> GetUserByUserName(string userName)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
    }
}
