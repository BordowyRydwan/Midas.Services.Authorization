using Application.Dto;
using Application.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FamilyController : ControllerBase
{
    private readonly IFamilyService _familyService;
    private readonly ILogger<FamilyController> _logger;

    public FamilyController(ILogger<FamilyController> logger, IFamilyService familyService)
    {
        _logger = logger;
        _familyService = familyService;
    }

    [SwaggerOperation(Summary = "Add new family")]
    [HttpPost("Add", Name = nameof(AddNewFamily))]
    [ProducesResponseType(typeof(AddNewFamilyReturnDto), 200)]
    public async Task<IActionResult> AddNewFamily(AddNewFamilyDto dto)
    {
        try
        {
            var returnDto = await _familyService.AddNewFamily(dto).ConfigureAwait(false);
            return Ok(returnDto);
        }
        catch (UserException)
        {
            _logger.LogError("Could not find user with id: {Id}. Process terminated.", dto.FounderId);
            return NotFound("Could not create a family with non-existing user");
        }
        catch (FamilyException ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest(ex.Message);
        }
    }
    
    [SwaggerOperation(Summary = "Delete specified family")]
    [HttpDelete("Delete", Name = nameof(DeleteFamily))]
    public async Task<IActionResult> DeleteFamily(ulong id)
    {
        var deleteSuccessful = await _familyService.DeleteFamily(id).ConfigureAwait(false);

        if (deleteSuccessful)
        {
            return Ok();
        }
        
        _logger.LogError("Could not find family with ID: {Id}", id);
        return NotFound();
    }
    
    [SwaggerOperation(Summary = "Add user to specified family")]
    [HttpPost("Add/User", Name = nameof(AddUserToFamily))]
    public async Task<IActionResult> AddUserToFamily(AddUserToFamilyDto dto)
    {
        var additionSuccessful = await _familyService.AddUserToFamily(dto).ConfigureAwait(false);
        
        if (additionSuccessful)
        {
            return Ok();
        }
        
        _logger.LogError("Could not add user with email: \"{Email}\" to family id: {Id}", dto.Email, dto.FamilyId);
        return NotFound();
    }
    
    [SwaggerOperation(Summary = "Delete user from specified family")]
    [HttpDelete("Delete/User", Name = nameof(DeleteUserFromFamily))]
    public async Task<IActionResult> DeleteUserFromFamily(DeleteUserFromFamilyDto dto)
    {
        var deletionSuccessful = await _familyService.DeleteUserFromFamily(dto).ConfigureAwait(false);
        
        if (deletionSuccessful)
        {
            return Ok();
        }
        
        _logger.LogError("Could not add user with email: \"{Email}\" to family id: {Id}", dto.Email, dto.FamilyId);
        return NotFound();
    }

    [SwaggerOperation(Summary = "Set a user role to specified family")]
    [HttpPatch("Set/UserRole", Name = nameof(SetUserFamilyRole))]
    public async Task<IActionResult> SetUserFamilyRole(SetUserFamilyRoleDto dto)
    {
        var settingSuccessful = await _familyService.SetUserFamilyRole(dto).ConfigureAwait(false);
        
        if (settingSuccessful)
        {
            return Ok();
        }
        
        _logger.LogError("Could not add user with email: \"{Email}\" to family id: {Id}", dto.Email, dto.FamilyId);
        return NotFound();
    }
}