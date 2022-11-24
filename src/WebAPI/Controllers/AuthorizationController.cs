using System.Text.Json;
using Application.Dto;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Midas.Services;
using Swashbuckle.AspNetCore.Annotations;
using IAuthorizationService = Application.Interfaces.IAuthorizationService;
using UserRegisterDto = Application.Dto.UserRegisterDto;
using UserUpdateEmailDto = Application.Dto.UserUpdateEmailDto;

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

    [SwaggerOperation(Summary = "Login a user")]
    [HttpPost("Login", Name = nameof(AuthorizeUser))]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> AuthorizeUser(UserLoginDto user)
    {
        var userCredentialsCheck = await _authorizationService.CheckUserCredentials(user).ConfigureAwait(false);

        if (!userCredentialsCheck)
        {
            _logger.LogError("Email or password are invalid.");
            return BadRequest();
        }

        var jwtToken = await _authorizationService.GenerateJwtToken(user).ConfigureAwait(false);
        return Ok(JsonSerializer.Serialize(jwtToken));
    }
    
    [SwaggerOperation(Summary = "Register new user")]
    [HttpPost("Register", Name = nameof(RegisterNewUser))]
    [ProducesResponseType(typeof(UserRegisterReturnDto), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> RegisterNewUser(UserRegisterDto user)
    {
        var registerReturnDto = await _authorizationService.RegisterNewUser(user).ConfigureAwait(false);

        if (registerReturnDto is not null)
        {
            return Ok(registerReturnDto);
        }

        _logger.LogError("Could not register user with email: {Email}", user.Email);
        return BadRequest();
    }
    
    [SwaggerOperation(Summary = "Update email address of existing user")]
    [HttpPatch("Update/Email", Name = nameof(UpdateUserEmail))]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> UpdateUserEmail(UserUpdateEmailDto user)
    {
        try
        {
            await _authorizationService.UpdateUserEmail(user).ConfigureAwait(false);
        }
        catch (UserException ex)
        {
            _logger.LogError(ex.Message + "\n" + ex.StackTrace);
            return BadRequest(ex.Message);
        }
        
        return Ok("User email updated");
    }
}