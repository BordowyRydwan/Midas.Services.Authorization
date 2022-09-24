using Application.Dto;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorizationController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<AuthorizationController> _logger;

    public AuthorizationController(ILogger<AuthorizationController> logger, IAuthorizationService authorizationService)
    {
        _logger = logger;
        _authorizationService = authorizationService;
    }

    [SwaggerOperation(Summary = "Register new user")]
    [HttpPost("Login", Name = nameof(AuthorizeUser))]
    [ProducesResponseType(typeof(string), 200)]
    public async Task<IActionResult> AuthorizeUser(UserLoginDto user)
    {
        var userCredentialsCheck = await _authorizationService.CheckUserCredentials(user).ConfigureAwait(false);

        if (!userCredentialsCheck)
        {
            _logger.LogError("Email or password are invalid.");
            return BadRequest();
        }

        var jwtToken = await _authorizationService.GenerateJwtToken(user).ConfigureAwait(false);
        return Ok(jwtToken);
    }
}