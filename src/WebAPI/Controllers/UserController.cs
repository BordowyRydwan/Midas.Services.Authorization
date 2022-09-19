using Application.Dto;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger, IUserService UserService)
    {
        _logger = logger;
        _userService = UserService;
    }

    [SwaggerOperation(Summary = "Register new user")]
    [HttpPost("Register", Name = nameof(RegisterNewUser))]
    [ProducesResponseType(typeof(UserRegisterReturnDto), 200)]
    public async Task<IActionResult> RegisterNewUser(UserRegisterDto user)
    {
        var registerReturnDto = await _userService.RegisterNewUser(user).ConfigureAwait(false);

        if (registerReturnDto is not null)
        {
            return Ok(registerReturnDto);
        }

        _logger.LogError("Could not register user with email: {Email}", user.Email);
        return BadRequest();
    }
    
    [SwaggerOperation(Summary = "Register new user")]
    [HttpPost("Authorize", Name = nameof(AuthorizeUser))]
    [ProducesResponseType(typeof(string), 200)]
    public async Task<IActionResult> AuthorizeUser(UserLoginDto user)
    {
        var userCredentialsCheck = await _userService.CheckUserCredentials(user).ConfigureAwait(false);

        if (!userCredentialsCheck)
        {
            _logger.LogError("Email or password are invalid.");
            return BadRequest();
        }

        var jwtToken = await _userService.GenerateJwtToken(user).ConfigureAwait(false);
        return Ok(jwtToken);
    }
}