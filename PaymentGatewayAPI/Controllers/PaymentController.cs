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
    public PaymentController(IDateTimeProvider dateTimeProvider, ApplicationDbContext context, IAuthenticationService authenticationService, IPaymentPaymentPublisher eventPublisher, IPaymentTransactionService transactionService)
    {
        _eventPublisher = eventPublisher;
        _transactionService = transactionService;
    }

    [HttpPost("ProcessPayment")]
    public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
    {

        var validator = new PaymentRequestValidator();
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return BadRequest(new { Errors = errors });
        }
        try
        {
            var correlationId = Guid.NewGuid().ToString();
            var eventResponse = await _eventPublisher.PublishPaymentEvent(request, correlationId);


            if (string.IsNullOrEmpty(eventResponse))
            {
                return StatusCode(500, new { Message = "No response received from the payment processor." });
            }

            var paymentResponse = JsonConvert.DeserializeObject<DemoPaymentResponse>(eventResponse);

            if (paymentResponse == null)
            {
                return StatusCode(500, new { Message = "Invalid response format from the payment processor." });
            }

            var transaction = await _transactionService.SaveTransaction(request, paymentResponse);

            return Ok(transaction);


        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while processing the payment.", Error = ex.Message });
        }
    }

    [HttpGet]
    [Route("Payments")]
    [Authorize]
    public IActionResult GetPayments()
    {
        return Ok("Payments");
    }
}
