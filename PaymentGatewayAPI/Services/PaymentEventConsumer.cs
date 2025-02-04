using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PaymentGatewayAPI.Configurations;
using PaymentGatewayAPI.DatabaseContext;
using PaymentGatewayAPI.Interfaces;
using PaymentGatewayAPI.Models;
using PaymentGatewayAPI.Requests;
using PaymentGatewayAPI.Responses;
using PaymentGatewayAPI.Validators;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Http;
using System.Text;
using System.Threading.Channels;
namespace PaymentGatewayAPI.Services;

public class PaymentEventConsumer : BackgroundService
{
    private readonly HttpClient _httpClient;
    private IConnection _connection;
    private readonly DemoPaymentAPISettings _settings;
    private readonly string _requestQueueName = "thirdPartyApiRequestQueue";
    private readonly string _responseQueueName = "thirdPartyApiResponseQueue";

    public PaymentEventConsumer(IConnection connection, HttpClient httpClient, IOptions<DemoPaymentAPISettings> settings)
    {
        _httpClient = httpClient;
        _connection = connection;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: _requestQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueDeclare(queue: _responseQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var request = JsonConvert.DeserializeObject<PaymentRequest>(message);
                if (request == null)
                {
                    throw new Exception("Invalid PaymentRequest format");
                }

                var paymentResponse = await CallThirdPartyPaymentAPI(message);

                var responseProperties = _channel.CreateBasicProperties();
                responseProperties.CorrelationId = ea.BasicProperties.CorrelationId;

                var responseBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(paymentResponse));

                _channel.BasicPublish(exchange: "", routingKey: _responseQueueName, basicProperties: responseProperties, body: responseBody);

                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _channel.BasicNack(ea.DeliveryTag, false, false);
            }
        };

        _channel.BasicConsume(queue: "thirdPartyApiRequestQueue", autoAck: false, consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task<DemoPaymentResponse?> CallThirdPartyPaymentAPI(string paymentData)
    {
        string thirdPartyApiUrl = _settings.APIUrl;

        var jsonContent = new StringContent(paymentData, Encoding.UTF8, "application/json");

        try
        {
            HttpResponseMessage response = await _httpClient.PostAsync(thirdPartyApiUrl, jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = new DemoPaymentResponse
                {
                    Status = "Failed",
                    Message = $"Failed to process payment. Status code: {response.StatusCode}, Reason: {response.ReasonPhrase}",
                    Timestamp = DateTime.UtcNow
                };

                return errorResponse;
            }

            var responseString = await response.Content.ReadAsStringAsync();

            var paymentResponse = JsonConvert.DeserializeObject<DemoPaymentResponse>(responseString);

            if (paymentResponse == null)
            {
                return new DemoPaymentResponse
                {
                    Status = "Failed",
                    Message = "No response data received from third-party API.",
                    Timestamp = DateTime.UtcNow
                };
            }
            return paymentResponse;
        }
        catch (Exception ex)
        {
            return new DemoPaymentResponse
            {
                Status = "Failed",
                Message = $"Error occurred while processing payment: {ex.Message}",
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
