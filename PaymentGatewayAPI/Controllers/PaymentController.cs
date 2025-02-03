using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PaymentGatewayAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        [HttpGet]
        [Route("Payments")]
        [Authorize]
        public IActionResult GetPayments()
        {
            return Ok("Payments");
        }
    }
}
