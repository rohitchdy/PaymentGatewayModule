using DemoPaymentAPI.Requests;
using DemoPaymentAPI.Validators;
using Microsoft.AspNetCore.Mvc;

namespace DemoPaymentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemoPaymentController : ControllerBase
    {
        private static readonly string[] Statuses = { "Success", "Pending", "Failed" };

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

            await Task.Delay(3000);
            Random random = new Random();

            // Randomly select a transaction status
            string status = Statuses[random.Next(Statuses.Length)];

            return Ok(new
            {
                Status = status,
                Message = "Mock payment processing complete.",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
