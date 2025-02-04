using Newtonsoft.Json;
using PaymentGatewayAPI.Interfaces;
using PaymentGatewayAPI.Requests;
using PaymentGatewayAPI.Responses;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace PaymentGatewayAPI.Services;

public class PaymentEventPublisher : IPaymentPaymentPublisher
{
    private readonly IModel _channel;
    private readonly string _requestQueueName = "thirdPartyApiRequestQueue";
    private readonly string _responseQueueName = "thirdPartyApiResponseQueue";

    public PaymentEventPublisher(IModel channel)
    {
        _channel = channel;
        _channel.QueueDeclare(queue: _requestQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueDeclare(queue: _responseQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public async Task<string> PublishPaymentEvent(PaymentRequest request, string correlationId)
    {
        var properties = _channel.CreateBasicProperties();
        properties.ReplyTo = _responseQueueName;
        properties.CorrelationId = correlationId;

        var message = JsonConvert.SerializeObject(request);

        _channel.BasicPublish(exchange: "", routingKey: _requestQueueName, basicProperties: properties, body: Encoding.UTF8.GetBytes(message));

        var tcs = new TaskCompletionSource<string>();
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            if (ea.BasicProperties.CorrelationId == correlationId)
            {
                var response = Encoding.UTF8.GetString(ea.Body.ToArray());
                tcs.TrySetResult(response);
            }
        };
        _channel.BasicConsume(queue: _responseQueueName, autoAck: true, consumer: consumer);
        return await tcs.Task;
    }
}
