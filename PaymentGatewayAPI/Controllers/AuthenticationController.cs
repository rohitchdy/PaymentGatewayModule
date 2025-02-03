using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using PaymentGatewayAPI.Interfaces;
using PaymentGatewayAPI.Requests;
using PaymentGatewayAPI.Services;

namespace PaymentGatewayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _tokenGenerator;
        public AuthenticationController(IAuthenticationService authenticationService, IPasswordHasher passwordHasher, IJwtTokenGenerator tokenGenerator)
        {
            _authenticationService = authenticationService;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            await Task.CompletedTask;
            var user = await _authenticationService.GetUserByUserName(loginRequest.UserName);
            if (user is null)
            {
                return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "User with the email not found.");
            }

            if (!_passwordHasher.VerifyPassword(loginRequest.Password, user.PasswordHash))
            {
                return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Invalid password.");
            }

            var token = _tokenGenerator.GenerateToken(user);
            var response = new AuthenticationResponse(user, token);
            return Ok(response);
        }
    }
}
