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
    public async Task<IActionResult> RegisterNewUser(UserRegisterDto user)
    {
        var isRegistrationSuccessful = await _userService.RegisterNewUser(user).ConfigureAwait(false);

        if (isRegistrationSuccessful)
        {
            return Ok();
        }

        _logger.LogError("Could not register user with email: {Email}", user.Email);
        return BadRequest();
    }
}