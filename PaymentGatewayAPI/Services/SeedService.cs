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

        var isCustomerExist = _context.Customers.Any();
        if (!isCustomerExist)
        {
            var user1 = await _context.Users.Where(u => u.UserName == "random_person").FirstOrDefaultAsync();
            var user2 = await _context.Users.Where(u => u.UserName == "unknown_person").FirstOrDefaultAsync();

            if (user1 != null && user2 != null)
            {
                await _context.Customers.AddRangeAsync(
                    new Customer
                    {
                        CustomerId = Guid.NewGuid(),
                        FullName = "Random Person",
                        Email = "random.persom@gmail.com",
                        CreatedOn = DateTime.UtcNow,
                        UserId = user1.UserId,
                        User = user1
                    },
                    new Customer
                    {
                        CustomerId = Guid.NewGuid(),
                        FullName = "Unknown Person",
                        Email = "unknowm.person@gmail.com",
                        CreatedOn = DateTime.UtcNow,
                        UserId = user2.UserId,
                        User = user2
                    });
                await _context.SaveChangesAsync();
            }
        }
    }
}
