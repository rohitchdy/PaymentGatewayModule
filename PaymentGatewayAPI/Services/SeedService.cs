using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PaymentGatewayAPI.DatabaseContext;
using PaymentGatewayAPI.Interfaces;
using PaymentGatewayAPI.Models;

namespace PaymentGatewayAPI.Services;

public class SeedService : ISeedService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    public SeedService(ApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }
    public async Task SeedAsync()
    {
        var isUserExist = _context.Users.Any();
        if (!isUserExist)
        {
            await _context.Users.AddRangeAsync(
                new User
                {
                    UserId = Guid.NewGuid(),
                    UserName = "random_person",
                    PasswordHash = _passwordHasher.GenerateHashPassword("pass@123"),
                    CreatedOn = DateTime.UtcNow
                },
                new User
                {
                    UserId = Guid.NewGuid(),
                    UserName = "unknown_person",
                    PasswordHash = _passwordHasher.GenerateHashPassword("pass@123"),
                    CreatedOn = DateTime.UtcNow
                });
            await _context.SaveChangesAsync();
        }
    }
}
