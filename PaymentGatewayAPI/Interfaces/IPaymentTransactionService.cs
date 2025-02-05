using PaymentGatewayAPI.DTOs;
using PaymentGatewayAPI.Models;
using PaymentGatewayAPI.Requests;
using PaymentGatewayAPI.Responses;

namespace PaymentGatewayAPI.Interfaces;

public interface IPaymentTransactionService
{
    Task<Transaction> SaveTransaction(PaymentRequest request, DemoPaymentResponse response);
    Task<List<TransactionDTO>> GetPayments(DateTime fromDate, DateTime toDate, string status, string userName);
}
