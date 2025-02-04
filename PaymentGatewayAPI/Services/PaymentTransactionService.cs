using PaymentGatewayAPI.DatabaseContext;
using PaymentGatewayAPI.Interfaces;
using PaymentGatewayAPI.Models;
using PaymentGatewayAPI.Requests;
using PaymentGatewayAPI.Responses;

namespace PaymentGatewayAPI.Services;

public class PaymentTransactionService : IPaymentTransactionService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IAuthenticationService _authenticationService;
    private readonly ApplicationDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public PaymentTransactionService(IAuthenticationService authenticationService, IDateTimeProvider dateTimeProvider, ApplicationDbContext dbContext)
    {
        _authenticationService = authenticationService;
        _dateTimeProvider = dateTimeProvider;
        _dbContext = dbContext;
    }

    public async Task<Transaction> SaveTransaction(PaymentRequest request, DemoPaymentResponse response)
    {

        using (var dbTransaction = await _dbContext.Database.BeginTransactionAsync())
        {
            try
            {
                var userId = await _authenticationService.GetCurrentUserId() ?? Guid.Empty;

                var transaction = new Transaction
                {
                    Amount = request.Amount,
                    Currency = request.Currency,
                    PaymentMode = request.PaymentMode,
                    FullName = request.FullName,
                    Email = request.Email,
                    CreatedOn = _dateTimeProvider.UtcNow,
                    UserId = userId,
                    Status = response.Status
                };

                _dbContext.Transactions.Add(transaction);
                await _dbContext.SaveChangesAsync();
                await dbTransaction.CommitAsync();
                return transaction;
            }
            catch (Exception)
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }
    }
}
