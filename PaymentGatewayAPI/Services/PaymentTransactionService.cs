using Microsoft.EntityFrameworkCore;
using PaymentGatewayAPI.DatabaseContext;
using PaymentGatewayAPI.DTOs;
using PaymentGatewayAPI.Interfaces;
using PaymentGatewayAPI.Models;
using PaymentGatewayAPI.Requests;
using PaymentGatewayAPI.Responses;

namespace PaymentGatewayAPI.Services;

public class PaymentTransactionService : IPaymentTransactionService
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ApplicationDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<PaymentTransactionService> _logger;

    public PaymentTransactionService(IAuthenticationService authenticationService, IDateTimeProvider dateTimeProvider, ApplicationDbContext dbContext, ILogger<PaymentTransactionService> logger)
    {
        _authenticationService = authenticationService;
        _dateTimeProvider = dateTimeProvider;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<List<TransactionDTO>> GetPayments(DateTime fromDate, DateTime toDate, string? status, string? userName)
    {
        var transactions = await (from txn in _dbContext.Transactions
                                  join user in _dbContext.Users on txn.UserId equals user.UserId
                                  where txn.CreatedOn.Date >= fromDate.Date && txn.CreatedOn.Date <= toDate.Date && (txn.Status == status || string.IsNullOrEmpty(status)) && (user.UserName == userName || string.IsNullOrEmpty(userName))
                                  select new TransactionDTO
                                  {
                                      TransactionId=txn.TransactionId,
                                      Status = txn.Status,
                                      Currency = txn.Currency,
                                      Amount = txn.Amount,
                                      TransactionDate = txn.CreatedOn,
                                      PaymentMode = txn.PaymentMode,
                                      CustomerName = txn.FullName,
                                      Email = txn.Email,
                                      UserName = user.UserName
                                  }).ToListAsync();

        return transactions;
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
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                await dbTransaction.RollbackAsync();
                throw;
            }
        }
    }
}
