using PaymentGatewayAPI.Models;
using PaymentGatewayAPI.Requests;
using PaymentGatewayAPI.Responses;

namespace PaymentGatewayAPI.Interfaces;

public interface IPaymentTransactionService
{
    Task<Transaction> SaveTransaction(PaymentRequest request, DemoPaymentResponse response);
}
