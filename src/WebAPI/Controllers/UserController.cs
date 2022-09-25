using Application.Dto;
using Application.Interfaces;
using Domain.Exceptions;
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
    
    [SwaggerOperation(Summary = "Update data of existing user")]
    [HttpPatch("Update/Data", Name = nameof(UpdateUserData))]
    public async Task<IActionResult> UpdateUserData(UserUpdateDto user)
    {
        var updateSuccess = await _userService.UpdateUserData(user).ConfigureAwait(false);

        if (updateSuccess)
        {
            return Ok();
        }

        _logger.LogError("Could not update data for user with email: {Email}", user.Email);
        return BadRequest();
    }
    
    [SwaggerOperation(Summary = "Update email address of existing user")]
    [HttpPatch("Update/Email", Name = nameof(UpdateUserEmail))]
    public async Task<IActionResult> UpdateUserEmail(UserUpdateEmailDto user)
    {
        try
        {
            await _userService.UpdateUserEmail(user).ConfigureAwait(false);
        }
        catch (UserException ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest(ex.Message);
        }
        
        return Ok();
    }
    
    // TODO: Fix security issue - now everyone has access to change everyone's password
    [SwaggerOperation(Summary = "Change password of existing user")]
    [HttpPatch("Update/Password", Name = nameof(UpdateUserPassword))]
    public async Task<IActionResult> UpdateUserPassword(UserUpdatePasswordDto user)
    {
        try
        {
            await _userService.UpdateUserPassword(user).ConfigureAwait(false);
        }
        catch (UserException ex)
        {
            _logger.LogError(ex.Message);
            return NotFound("Could not find user with email: " + user.Email);
        }
        catch (PasswordException ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest(ex.Message);
        }

        return Ok();
    }
    
    [SwaggerOperation(Summary = "Change password of existing user")]
    [HttpGet("Email/{email}", Name = nameof(GetUserByEmail))]
    [ProducesResponseType(typeof(UserDto), 200)]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        var user = await _userService.GetUserByEmail(email).ConfigureAwait(false);

        if (user is not null)
        {
            return Ok(user);
        }

        _logger.LogError("Could not find user with email: " + user.Email);
        return NotFound();
    }
}