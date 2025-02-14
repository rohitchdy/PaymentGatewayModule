using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentGatewayAPI.DatabaseContext;
using PaymentGatewayAPI.Interfaces;
using PaymentGatewayAPI.Requests;
using PaymentGatewayAPI.Responses;
using PaymentGatewayAPI.Validators;
using Newtonsoft.Json;

namespace PaymentGatewayAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentPaymentPublisher _eventPublisher;
    private readonly IPaymentTransactionService _transactionService;
    private readonly ILogger<PaymentController> _logger;
    private readonly IEmailService _emailService;
    public PaymentController(IDateTimeProvider dateTimeProvider, ApplicationDbContext context, IAuthenticationService authenticationService, IPaymentPaymentPublisher eventPublisher, IPaymentTransactionService transactionService, ILogger<PaymentController> logger, IEmailService emailService)
    {
        _eventPublisher = eventPublisher;
        _transactionService = transactionService;
        _logger = logger;
        _emailService = emailService;
    }

    [HttpPost("ProcessPayment")]
    public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
    {

        var validator = new PaymentRequestValidator();
        var validationResult = validator.Validate(request);



        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            _logger.LogError(string.Join(",", errors));
            return BadRequest(new { Errors = errors });
        }


        try
        {
            var correlationId = Guid.NewGuid().ToString();
            var eventResponse = await _eventPublisher.PublishPaymentEvent(request, correlationId);


            if (string.IsNullOrEmpty(eventResponse))
            {
                _logger.LogError(eventResponse);
                return StatusCode(500, new { Message = "No response received from the payment processor." });
            }

            var paymentResponse = JsonConvert.DeserializeObject<DemoPaymentResponse>(eventResponse);

            if (paymentResponse == null)
            {
                _logger.LogError($"Payment response is null");
                return StatusCode(500, new { Message = "Invalid response format from the payment processor." });
            }

            var transaction = await _transactionService.SaveTransaction(request, paymentResponse);

            if (paymentResponse.Status == "Failed")
            {
                throw new Exception("Payment not success.");
            }
            await _emailService.SendEmailAsync(request.Email, "Payment Transaction", "Payment Success");
            return Ok(transaction);


        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while processing the payment.", ex.Message);
            return StatusCode(500, new { Message = "An error occurred while processing the payment.", Error = ex.Message });
        }
    }

    [HttpGet]
    [Route("Payments")]
    [Authorize]
    public async Task<IActionResult> GetPayments(DateTime fromDate, DateTime toDate, string? status, string? userName)
    {
        var transactions = await _transactionService.GetPayments(fromDate, toDate, status, userName);
        return Ok(transactions);
    }
}
