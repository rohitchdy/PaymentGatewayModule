using PaymentGatewayAPI.Requests;
namespace PaymentGatewayAPI.Interfaces;

public interface IPaymentPaymentPublisher
{
    Task<string> PublishPaymentEvent(PaymentRequest request, string correlationId);
}
